using BlueMoon.MVC.Controls;
using BlueMoon.DynWeb.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Linq;
using System.Web.Script.Serialization;
using BlueMoon.DynWeb.Models;
using BlueMoon.DynWeb.Entities;
using Action = BlueMoon.DynWeb.Entities.Action;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace BlueMoon.DynWeb.Controllers
{
    static class Common
    {
        public static readonly Regex s_GetRootColName = new Regex(@"\[I\]\.\[(.*?)\]", RegexOptions.Compiled);
        public static string ToValueId(this string val, string jsonArray)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var refData = ser.Deserialize<object[]>(jsonArray);
            foreach (dynamic v in refData)
            {
                if (v["text"].ToString().ToLower() == val.ToLower()) return v["valueId"] + "";
            }
            return null;
        }
        public static string ToText(this string valId, object[] refData)
        {
            if (!string.IsNullOrEmpty(valId))
            {
                foreach (dynamic v in refData)
                {
                    if (v["valueId"].ToString().ToLower() == valId.ToLower()) return v["text"] + "";
                }
            }
            
            return "[Invalid value]";
        }
        public static void UpdateToValueId(this Query[] queries, ModelDefinition props)
        {
            if (queries == null) return;
            for (int i = 0; i < queries.Length; i++)
            {
                var p = queries[i];
                var colName = p.propId;
                colName = Common.s_GetRootColName.Replace(colName, "$1");
                if (props[colName] != null && props[colName].DataSourceField != null && props[colName].DataSourceField.StartsWith("["))
                {
                    string valueId = p.value.ToValueId(props[colName].DataSourceField);
                    if (valueId != null) p.value = valueId;

                }
            }
        }
        public static int ManipulateDataList(List<ModelData> items, ModelDefinition properties, string displayPropName)
        {
            if (items == null) return 0;
            int totalRow = 0;
            items.ForEach(p =>
            {

                totalRow = (int)p["TotalRows"];
                p.Remove("Type");
                p.Remove("TotalRows");
                p.Remove("RowNum");

                foreach (var prop in properties)
                {
                    if (!prop.AllowShowGrid && prop.Name != "ID" && prop.Name != displayPropName)
                    {
                        p.Remove(prop.Name);
                    }
                    else if (!string.IsNullOrEmpty(prop.DisplayFormat))
                    {
                        p[prop.Name] = string.Format(prop.DisplayFormat, p[prop.Name]);
                    }
                }

            });
            return totalRow;
        }
        public static void ManipulateDataDetail(ModelEntity detail)
        {
            var properties = detail.Properties;
            foreach (var prop in properties)
            {
                if (!string.IsNullOrEmpty(prop.DisplayFormat))
                {
                    detail[prop.Name+"_Display"] = string.Format(prop.DisplayFormat, detail[prop.Name]);
                }
            }
        }
        public static ApprovalRule GetMatchedApprovalRule(int type, int itemId)
        {
            var rules = GetApprovalRules(type);
            if (rules != null)
                foreach (var r in rules)
                {
                    if (GetMatchedApprovalList(new ApprovalRule[] { r }, itemId) != null) return r;
                }

            return null;
        }
        public static ApprovalRule[] GetApprovalRules(int type)
        {
            ApprovalRule rule = new ApprovalRule();
            var rules = rule.GetAll(null, "[AppliedItemType] = " + type);
            
            if (rules != null)
            {
                List<ApprovalRule> appliedRules = new List<ApprovalRule>();
                foreach (var r in rules)
                {
                    if (SessionManager.CurrentUser.HasPermission(r.Permissions.Split(new char[] { ',', ';' })))
                    {
                        appliedRules.Add(r);
                    }
                }
                if (appliedRules.Count > 0) return appliedRules.ToArray();
            }
            return null;
        }
        public static List<ModelData> GetMatchedApprovalList(ApprovalRule[] rules, int itemId, int pageIndex = 1, int pageSize = 1)
        {
            List<string> states = new List<string>();
            List<Query[]> queriesList = new List<Query[]>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            int type = 0;
            ModelEntity itemDetail = null;
            foreach (var rule in rules)
            {
                if (type == 0)
                {
                    type = rule.AppliedItemType;
                    itemDetail = CacheManager.CreateModelEntity(type);
                }
                states.Add(rule.QueryState);
                var queries = serializer.Deserialize<Query[]>(rule.Queries);
                queries.UpdateToValueId(itemDetail.Properties);
                queriesList.Add(queries);
            }
            
            return itemDetail.GetApprovalList(states.ToArray(), queriesList.ToArray(), itemId, pageSize, pageIndex);
        }
        public static List<ModelData> GetData(ItemType itemType, string searchFor = "", string state = "", int page = 1, int pageSize = 15)
        {

            ModelEntity item = CacheManager.CreateModelEntity(itemType.ID);
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(state))
            {
                if (state == "submitted") sb.Append("(R.[State] = 'submitted' OR  (R.[State] <> 'approved' AND R.[State] <> 'rejected')) AND ");
                else sb.Append(string.Format("R.[State] = '{0}' AND ", state.AntiSQLInjection()));
            }
            else if (itemType.ApprovalProcess)
            {
                sb.Append("(R.[State] = '' OR R.[State] IS NULL) AND ");
            }
            if (itemType.ListByCreator)
            {
                sb.Append(string.Format("R.[CreatedBy] = {0} AND ", SessionManager.CurrentUser.ID));
            }

            sb.Append(string.Format("(I.[{0}] LIKE N'%{1}%'", itemType.DisplayProperty, searchFor.AntiSQLInjection()));

            foreach (var prop in item.Properties)
            {
                if (prop.AllowSearch)
                {
                    var ds = prop.DataSourceField.ToDataSourceName();
                    if (string.IsNullOrEmpty(ds))
                    {
                        ds = prop.DataSourceField;
                        if (string.IsNullOrEmpty(ds)) sb.Append(string.Format(" OR I.[{0}] LIKE N'%{1}%'", prop.Name, searchFor.AntiSQLInjection()));
                        else
                        {
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var refData = ser.Deserialize<object[]>(ds);
                            foreach (dynamic v in refData)
                            {
                                var vv = (string)v["text"].ToString().ToLower();
                                if (searchFor.ToLower().Contains(vv)) sb.Append(string.Format(" OR I.[{0}] = N'{1}'", prop.Name, v["valueId"]));
                            }

                        }

                    }
                    else
                    {
                        ItemType refItemType = CacheManager.AllItemTypes[ds];
                        sb.Append(string.Format(" OR [{2}].[{0}] LIKE N'%{1}%'", refItemType.DisplayProperty, searchFor.AntiSQLInjection(), prop.Name));
                    }
                }
            }
            sb.Append(")");
            return item.SearchItems(sb.ToString(), pageSize, page);
        }

        
        public static dynamic GetItemList(string type, string searchFor = "", string state = "", int page = 1, int pageSize = 15)
        {
            var itemType = CacheManager.AllItemTypes[type];
            var modelProperties = CacheManager.GetModelDef(itemType.ID);
            var data = GetData(itemType, searchFor, state, page, pageSize);
            if (data == null) return new { };
            int totalRow = ManipulateDataList(data, modelProperties, itemType.DisplayProperty);
            return new { items = new { data, pageIndex = page, pageSize, totalRow } }; 
        }
    }
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [AuthorizeUser(Permission = "[ItemAccess]")]
    public class ApiController : Controller
    {
        #region Items
        [Route("~/api/item/list")]
        public ActionResult GetList(string type, string searchFor = "", int page = 1, int pageSize = 15)
        {
            return Json(Common.GetItemList(type, searchFor, null, page, pageSize));
        }
        [Route("~/api/item/list/submitted")]
        public ActionResult GetListSubmitted(string type, string searchFor = "", int page = 1, int pageSize = 15)
        {
            return Json(Common.GetItemList(type, searchFor, ApprovalState.SUBMITTED, page, pageSize));
        }
        [Route("~/api/item/list/approved")]
        public ActionResult GetListApproved(string type, string searchFor = "", int page = 1, int pageSize = 15)
        {
            return Json(Common.GetItemList(type, searchFor, ApprovalState.APPROVED, page, pageSize));
        }
        [Route("~/api/item/list/rejected")]
        public ActionResult GetListRejected(string type, string searchFor = "", int page = 1, int pageSize = 15)
        {
            return Json(Common.GetItemList(type, searchFor, ApprovalState.REJECTED, page, pageSize));
        }
        [AuthorizeUser(Permission = "[ModAccess]")]
        [Route("~/api/item/save")]
        public ActionResult Save(string type, ModelData data, int parentID = 0)
        {
            if (parentID > 0)
            {
                Item parentItem = new Item();
                parentItem.ID = parentID;
                parentItem.Get();
                ItemType parentItemType = CacheManager.AllItemTypes[parentItem.Type];
                bool blockEditable = parentItemType.ApprovalProcess && !string.IsNullOrEmpty(parentItem.State);
                if (blockEditable) return Unauthorized();
            }
            ItemType itemType = CacheManager.AllItemTypes[type];
            data.Type = itemType.ID;


            Item item = new Item();
            ModelEntity itemDetail = CacheManager.CreateModelEntity(data.Type);
            string msg = null;
            List<dynamic> diffFields = null;
            if (data.ID > 0)
            {

                item.ID = data.ID;
                item.Get();
                
                item.UpdatedTime = DateTime.Now;
                item.UpdatedBy = SessionManager.CurrentUser.ID;
                item.Update();

                itemDetail["ID"] = item.ID;
                if (itemType.TrackingChange)
                {
                    itemDetail.Get();
                    ModelEntity newVals = new ModelEntity(data.Type, itemDetail.Properties);
                    newVals["ID"] = item.ID;
                    newVals.Copy(data);
                    diffFields = itemDetail.Differ(newVals);
                }
                itemDetail.Copy(data);
                itemDetail.Update();
                msg = itemType.Name + " is updated";
            }

            else // add new item
            {
                item.Type = data.Type;
                item.CreatedTime = DateTime.Now;
                item.CreatedBy = SessionManager.CurrentUser.ID;
                item.UpdatedBy = 0;
                item.UpdatedTime = DateTime.Now;
                item.Insert();

                if (parentID > 0)
                {
                    ItemRelation itemRelation = new ItemRelation();
                    itemRelation.IDChild = item.ID;
                    itemRelation.IDParent = parentID;
                    itemRelation.Description = "";
                    itemRelation.Insert();
                    msg = itemType.Name + " as a child";
                }
                itemDetail.Copy(data);
                itemDetail["ID"] = item.ID;
                itemDetail.Insert();
                if (msg == null) msg = itemType.Name;
                msg += " is inserted";
            }
            //log tracking
            if (diffFields != null)
            {
                TrackingChange tracking = new TrackingChange();
                tracking.ItemId = item.ID;
                tracking.UpdatedBy = SessionManager.CurrentUser.Username;
                tracking.UpdatedTime = DateTime.Now.ToUniversalTime();
                var props = itemDetail.Properties;
                foreach (var p in diffFields)
                {
                    tracking.Field = p.Field;
                    var ds = props[tracking.Field].DataSourceField.ToDataSourceName();
                    if (string.IsNullOrEmpty(ds))
                    {
                        if (string.IsNullOrEmpty(props[tracking.Field].DataSourceField))
                        {
                            tracking.OldValue = p.OldValue.ToString();
                            tracking.NewValue = p.NewValue.ToString();
                        }
                        else
                        {
                            JavaScriptSerializer ser = new JavaScriptSerializer();
                            var refData = ser.Deserialize<object[]>(props[tracking.Field].DataSourceField);
                            tracking.OldValue = p.OldValue.ToString();
                            if(tracking.OldValue != "") tracking.OldValue = tracking.OldValue.ToText(refData);
                            tracking.NewValue = p.NewValue.ToString();
                            if (tracking.NewValue != "")  tracking.NewValue = tracking.NewValue.ToText(refData);
                        }
                        
                    }
                    else
                    {
                        var refItemType = CacheManager.AllItemTypes[ds];

                        ModelEntity refItemDetail = CacheManager.CreateModelEntity(refItemType.ID);

                        refItemDetail["ID"] = p.OldValue;
                        refItemDetail.Get();
                        tracking.OldValue = refItemDetail[refItemType.DisplayProperty].ToString();

                        refItemDetail["ID"] = p.NewValue;
                        refItemDetail.Get();
                        tracking.NewValue = refItemDetail[refItemType.DisplayProperty].ToString();

                    }
                    tracking.Field = props[tracking.Field]?.Label;//convert field name to label
                    tracking.Insert();
                }
                

                
                //???
            }
            return Json(new { msg });
        }
        [AuthorizeUser(Permission = "[ModAccess]")]
        [Route("~/api/item/delete")]
        public ActionResult Delete(string type, int ID)
        {
            ItemType itemType = CacheManager.AllItemTypes[type];
            ModelEntity item = CacheManager.CreateModelEntity(itemType.ID);
            return Json(new { result = item.Delete(ID) });
        }
        [AllowAnonymous] //check permission depend on ID
        [Route("~/api/item/get")]
        public ActionResult GetDetail(string type, int ID = 0)
        {
            ItemType itemType = CacheManager.AllItemTypes[type];
            ModelEntity itemDetail = CacheManager.CreateModelEntity(itemType.ID);
            if (itemType.TypeOfItem == (int)TypeOfItem.ChildOnly)
            {
                ItemRelation rel = new ItemRelation();
                rel.Get("[IDChild] = " + ID);
                if (!(
                        SessionManager.CurrentUser.IsLinkedItem(rel.IDParent)
                        ||
                        SessionManager.CheckItemPermission(itemType) && SessionManager.CheckItemPermission(rel.IDParent)
                    )
                ) return Unauthorized();
            }
            else
            {
                if (!(SessionManager.CurrentUser.IsLinkedItem(ID) || SessionManager.CheckItemPermission(itemType))) return Unauthorized();
            }
            itemDetail["ID"] = ID;
            itemDetail.Get();
            Common.ManipulateDataDetail(itemDetail);
            return Json(itemDetail);
        }
        [AllowAnonymous]
        [Route("~/api/item/childList")]
        public ActionResult GetChildList(string type, int parentID, int page = 1, int pageSize = 7)
        {
            if (!SessionManager.CheckItemPermission(parentID)) return Unauthorized();
            ItemType itemType = CacheManager.AllItemTypes[type];
            ModelEntity item = CacheManager.CreateModelEntity(itemType.ID);
            List<ModelData> items = item.GetChildList(pageSize, page, parentID);
            if (items == null) return Json(new { });
            int totalRow = Common.ManipulateDataList(items, item.Properties, itemType.DisplayProperty);
            return Json(new { items = new { data = items, pageIndex = page, pageSize, totalRow } });
        }
        [AllowAnonymous]
        [Route("~/api/item/selectChildItems")]
        public ActionResult GetAvailableSelectionList(string type, int parentID, int page = 1, int pageSize = 5)
        {
            if (!SessionManager.CheckItemPermission(parentID)) return Unauthorized();
            ItemType itemType = CacheManager.AllItemTypes[type];
            ModelEntity item = CacheManager.CreateModelEntity(itemType.ID);
            List<ModelData> items = item.SelectChildItems(parentID, "", 0, pageSize, page);
            if (items == null) return Json(new { });
            int totalRow = Common.ManipulateDataList(items, item.Properties, itemType.DisplayProperty);
            return Json(new { items = new { data = items, pageIndex = page, pageSize, totalRow } });
        }
        [AuthorizeUser(Permission = "[ModAccess]")]
        [Route("~/api/item/addChildItems")]
        public ActionResult AddToChildList(int parentID, int[] selectedItems)
        {
            Item item = new Item();
            item.ID = parentID;
            item.Get();
            ItemType itemType = CacheManager.AllItemTypes[item.Type];
            bool blockEditable = itemType.ApprovalProcess && !string.IsNullOrEmpty(item.State);
            if(blockEditable) return Unauthorized();

            ItemRelation itemRelation = new ItemRelation();
            itemRelation.IDParent = parentID;
            foreach(var id in selectedItems)
            {
                itemRelation.IDChild = id;
                itemRelation.Description = "";
                itemRelation.Insert();
            }

            return Json(new { result = true });
        }
        [AuthorizeUser(Permission = "[ModAccess]")]
        [Route("~/api/item/removeChildItem")]
        public ActionResult RemoveFromChildList(int parentID, int childID)
        {
            Item item = new Item();
            item.ID = parentID;
            item.Get();
            ItemType itemType = CacheManager.AllItemTypes[item.Type];
            bool blockEditable = itemType.ApprovalProcess && !string.IsNullOrEmpty(item.State);
            if (blockEditable) return Unauthorized();

            Item childItem = new Item();
            childItem.ID = childID;
            childItem.Get();
            if (CacheManager.AllItemTypes[childItem.Type].TypeOfItem == (int)TypeOfItem.ChildOnly)
            {
                

                ModelEntity itemDetail = CacheManager.CreateModelEntity(childItem.Type);
                itemDetail["ID"] = childID;
                itemDetail.Delete();

                childItem.Delete();
            }

            ItemRelation itemRelation = new ItemRelation();
            itemRelation.IDParent = parentID;
            itemRelation.IDChild = childID;
            
            return Json(new { result = itemRelation.Delete() });
        }
        [Route("~/api/item/getChildItems")]
        public ActionResult GetChildItems(int parentID)
        {
            Item item = new Item();
            item.ID = parentID;
            item.Get();
            ItemType itemType = CacheManager.AllItemTypes[item.Type];
            bool blockEditable = itemType.ApprovalProcess && !string.IsNullOrEmpty(item.State);

            if (!(SessionManager.CheckItemPermission(itemType) || SessionManager.CurrentUser.IsLinkedItem(parentID))) return Unauthorized();
            // get list childitems
            ItemTypeRelation itemChild = new ItemTypeRelation();
            List<ItemTypeRelation> lstItemChild = itemChild.GetListChildItem(item.Type);
            Dictionary<string, object> lstChildItemModel = new Dictionary<string, object>();
            
            if (lstItemChild != null)
            {
                
                for (int i = 0; i < lstItemChild.Count; i++)
                {
                    
                    ItemType childItemType = CacheManager.AllItemTypes[lstItemChild[i].ChildTypeID];
                    if (!(SessionManager.CheckItemPermission(childItemType) || SessionManager.CurrentUser.IsLinkedItem(parentID))) continue;
                    /*
                    //ChildItemModel childItemModel = new ChildItemModel();
                    //childItemModel.ID = childItemType.ID;
                    //childItemModel.Name = childItemType.Name;
                    //childItemModel.Display = childItemType.Display;
                    //childItemModel.Type = lstItemChild[i].Type;
                    //childItemModel.Alias = lstItemChild[i].Alias;
                    */
                    int linkedItemId = 0;
                    if(lstItemChild[i].Type==2)//extension item
                    {

                        ModelEntity childItem = CacheManager.CreateModelEntity(lstItemChild[i].ChildTypeID);
                        List<ModelData> childItems = childItem.GetChildList(1, 1, parentID);
                        if (childItems != null && childItems.Count > 0)
                        {
                            linkedItemId = childItems[0].ID;
                        }
                        else
                        {
                            item = new Item();
                            item.Type = lstItemChild[i].ChildTypeID;
                            item.CreatedTime = DateTime.Now;
                            item.CreatedBy = 0;
                            item.UpdatedBy = 0;
                            item.UpdatedTime = DateTime.Now;
                            item.Insert();

                            ItemRelation itemRelation = new ItemRelation();
                            itemRelation.IDChild = item.ID;
                            itemRelation.IDParent = parentID;
                            itemRelation.Description = "";
                            itemRelation.Insert();

                            ModelEntity itemDetail = CacheManager.CreateModelEntity(lstItemChild[i].ChildTypeID);
                            itemDetail["ID"] = item.ID;
                            itemDetail.Insert();

                            linkedItemId = item.ID;
                        }

                        lstChildItemModel.Add("tab_" + childItemType.Name, new
                        {
                            title = string.IsNullOrEmpty(lstItemChild[i].Alias) ? childItemType.Display : lstItemChild[i].Alias,
                            itemName = childItemType.Name,
                            linkedMode = lstItemChild[i].Type,
                            idField = "ID",
                            displayField = childItemType.DisplayProperty,
                            itemId = linkedItemId,
                            editable = !blockEditable && SessionManager.CheckItemPermission(childItemType, true)
                        });
                    }
                    else
                    {
                        lstChildItemModel.Add("tab_" + childItemType.Name, new
                        {
                            title = string.IsNullOrEmpty(lstItemChild[i].Alias) ? childItemType.Display : lstItemChild[i].Alias,
                            itemName = childItemType.Name,
                            linkedMode = lstItemChild[i].Type,
                            idField = "ID",
                            displayField = childItemType.DisplayProperty,
                            showComment = childItemType.AllowComment,
                            showHistory = childItemType.TrackingChange,
                            editable = !blockEditable && SessionManager.CheckItemPermission(childItemType, true)
                        });
                    }
                    
                }
                
                
            }
            if (itemType.AllowFileAttachment)
            {
                lstChildItemModel.Add("tab_ItemAttachments", new
                {
                    title = "Attachments",
                    linkedMode = 3,//for attachment
                    itemId = parentID
                });
            }
            if (lstChildItemModel.Count > 0) return Json(new { linkedItems = lstChildItemModel });
            else return Json(new { });
        }
        [AllowAnonymous]
        [Route("~/api/item/refList")]
        public ActionResult GetRefList(string type, int parentID = 0, int itemId = 0,  string searchFor = "", Dictionary<string, object> filterBy = null)
        {

            ItemType itemType = CacheManager.AllItemTypes[type];
            ModelEntity item = CacheManager.CreateModelEntity(itemType.ID);
            string filterString = string.Format("[{0}] LIKE N'%{1}%'", itemType.DisplayProperty, searchFor.AntiSQLInjection());
            if (filterBy != null && filterBy.Count > 0)
            {
                foreach(var kp in filterBy)
                {
                    if (item.Properties.Exists(p => p.Name.ToLower() == kp.Key.ToLower())) filterString += string.Format(" AND [{0}] = N'{1}'", kp.Key, kp.Value);
                }
            }
            List<ModelData> items = item.GetRefItems(filterString, parentID, itemId, 256,  1);
            if(items==null) return Json(new { });
            items.ForEach(p =>
            {
                p["text"] = p[itemType.DisplayProperty];
                p["valueId"] = p.ID;
                p.Remove("Type");
            });
            items.Insert(0, new ModelData());
            return Json(items);
        }

        [Route("~/api/item/properties")]
        public ActionResult GetProps(int typeId = 0, byte mode = 0)
        {
            if (typeId == 0) return Json(new { });
            ItemType itemType = new ItemType();
            var cols = itemType.GetAllCols(typeId, mode);
            List<object> lst = new List<object>();
            cols.ForEach(p =>
            {
                lst.Add(new { text = p.Label, valueId = p.ColName });
            });
            return Json(lst);
        }
        #endregion
        #region Comment
        [AllowAnonymous]
        [Route("~/api/comment/list")]
        public ActionResult GetCommentList(int itemId, int page = 1, int pageSize = 5)
        {
            if (!SessionManager.CheckItemPermission(itemId)) return Unauthorized();
            Comment comment = new Comment();
            var comments = comment.GetListComment(itemId, pageSize, page);
            if (comments != null)
            {
                int totalRow = comments[0].TotalRows;

                return Json(new { items = new { data = comments, pageIndex = page, pageSize, totalRow } });
            }
            else return Json(new { });
            
        }
        [Route("~/api/comment/add")]
        public ActionResult AddComment(int itemId, string content)
        {
            if (!SessionManager.CheckItemPermission(itemId, true)) return Unauthorized();
            Comment model = new Comment();
            model.ItemID = itemId;
            model.Content = content;
            model.CreatedDate = DateTime.UtcNow;
            model.AccountID = SessionManager.CurrentUser.ID;
            return Json(new { result = model.Insert() });
        }
        #endregion
        [AllowAnonymous]
        [Route("~/api/history/list")]
        public ActionResult GetHistoryList(int itemId)
        {
            if (!SessionManager.CheckItemPermission(itemId)) return Unauthorized();
            TrackingChange trackingObj = new TrackingChange();
            trackingObj.ItemId = itemId;
            var list = trackingObj.GetAll("[UpdatedTime] DESC", "[ItemId] = " + itemId);
            if (list == null) return Json(new { });
            var result = list.GroupBy(i => new
            {
                i.ItemId,
                i.UpdatedBy,
                i.UpdatedTime
            }).Select(g => new {
                g.Key.UpdatedBy,
                g.Key.UpdatedTime,
                Changes = g.ToList()
            });

            
            return Json(new { histories = result });
        }

        [AuthorizeUser(Permission = Permission.MANAGE_SYSTEM)]
        [Route("~/api/item/types")]
        public ActionResult GetItemTypes()
        {
            List<object> lst = new List<object>();
            lst.Add(new { text = "", value = "" });
            CacheManager.AllItemTypes.Where(t => t.TypeOfItem==(int)TypeOfItem.Normal && !t.ListByCreator && !t.ApprovalProcess).ToList().ForEach(t =>
            {
                lst.Add(new { text = t.Display, valueId = t.ID });
            });
            return Json(lst);
        }
        [AuthorizeUser(Permission = Permission.MANAGE_SYSTEM)]
        [Route("~/api/item/selectionList")]
        public ActionResult GetMappingList(int typeId, string searchFor = "", int page = 1, int pageSize = 15)
        {
            var itemType = CacheManager.AllItemTypes[typeId];
            var modelProperties = CacheManager.GetModelDef(typeId);
            var data = Common.GetData(itemType, searchFor, null, page, pageSize);

            if (data == null) return Json(new { items = (string)null });
            int totalRow = 0;
            data.ForEach(p =>
            {

                totalRow = (int)p["TotalRows"];
                p.Remove("Type");
                p.Remove("TotalRows");
                p.Remove("RowNum");
                p["itemName"] = p[itemType.DisplayProperty];
                p["itemId"] = p["ID"];

                //cleanup
                foreach (var prop in modelProperties)
                {
                    p.Remove(prop.Name);
                }

            });
            return Json(new { items = new { data, pageIndex = page, pageSize, totalRow } });
        }
    }
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [AuthorizeUser(Permissions = new string[] { Permission.ACCESS_REPORTING, Permission.MANAGE_REPORTING })]
    public class ReportingApiController : Controller
    {

        #region Manage reports
        [AuthorizeUser(Permission = Permission.MANAGE_REPORTING)]
        [Route("~/api/report/detail")]
        public ActionResult GetReportDetail(int reportId)
        {
            Report report = new Report();
            report.ID = reportId;
            report.Get();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string[] cols = serializer.Deserialize<string[]>(report.DisplayColumns);
            Query[] queries = serializer.Deserialize<Query[]>(report.Queries);
            OrderBy[] sorts = serializer.Deserialize<OrderBy[]>(report.Sorts ?? "null");
            return Json(new { id = report.ID, name = report.Name, dataSourceId = report.DataSource, queries, colIds = cols, sorts });
        }
        [AuthorizeUser(Permission = Permission.MANAGE_REPORTING)]
        [Route("~/api/report/datasource")]
        public ActionResult GetDataSource()
        {
            List<object> lst = new List<object>();
            lst.Add(new { text = "", valueId = "" });
            CacheManager.AllItemTypes.Where(t => t.AllowReport).ToList().ForEach(t =>
            {
                lst.Add(new { text = t.Display, valueId = t.ID });
            });
            return Json(lst);
        }
       
        dynamic GetData(int dataSource, string[] cols, Query[] queries, OrderBy[] orderBys, int page = 1, int pageSize = 5)
        {
            ItemType itemType = new ItemType();
            var colDef = itemType.GetAllCols(dataSource);
            var props = CacheManager.CreateModelEntity(dataSource).Properties;
            Report report = new Report();
            List<string> selectedCols = new List<string>();
            for (int i = 0; i < cols.Length; i++)
            {
                var col = colDef.Find(cd => cd.ColName == cols[i]);
                if (col == null) continue;
                
                string colName = Common.s_GetRootColName.Replace(col.ColName, "$1");
                //manipulate value for ref items
                if (props[colName] != null && props[colName].DataSourceField != null && props[colName].DataSourceField.StartsWith("["))
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    var refData = ser.Deserialize<object[]>(props[colName].DataSourceField);
                    colName = "CASE " + col.ColName;
                    foreach (dynamic v in refData)
                    {
                        colName += " WHEN '" + v["valueId"] + "' THEN '" + v["text"] + "'";
                    }
                    colName += " END AS " + col.Alias;
                    selectedCols.Add(colName);
                }
                else selectedCols.Add(col.ColName + " " + col.Alias);
            }

            //manipulate query values
            queries.UpdateToValueId(props);

            string orderBy = null;
            if (orderBys != null && orderBys.Length > 0)
            {
                for (int i = 0; i < orderBys.Length; i++)
                {
                    var col = colDef.Find(cd => cd.ColName == orderBys[i].propId);
                    if (col == null) continue;
                    orderBys[i].propId = col.Alias;
                }
                orderBy = string.Join<OrderBy>(",", orderBys);
            }
            var items = report.GetData(dataSource, selectedCols.ToArray(), queries, orderBy, page, pageSize);
            int totalRow = 0;
            if (items != null)
            {
                items.ForEach(p =>
                {
                    totalRow = (int)p["TotalRows"];
                    p.Remove("Type");
                    p.Remove("TotalRows");
                    p.Remove("RowNum");

                    foreach (var prop in props)
                    {
                        if (!string.IsNullOrEmpty(prop.DisplayFormat))
                        {
                            p[prop.Name] = string.Format(prop.DisplayFormat, p[prop.Name]);
                        }
                    }
                });
            }

            List<object> columns = new List<object>();


            foreach (var c in cols)
            {
                var col = colDef.Find(cd => cd.ColName == c);
                if (col == null) continue;
                columns.Add(new { headerLabel = col.Label, dataField = col.Alias.Trim(new char[] { '[', ']' }) });
            }
            return new { data = items, pageIndex = page, pageSize, totalRow, options = new { columns } };
        }

        [Route("~/api/report/preview")]
        public ActionResult GetReportPreview(int dataSourceId, string[] colIds, Query[] queries, OrderBy[] sorts, int page = 1, int pageSize = 5)
        {
            if (colIds == null || colIds.Length == 0) return Json(new { msg = "Please select a column" });
            return Json(new { result = GetData(dataSourceId, colIds, queries, sorts, page, pageSize) });
        }
        [AuthorizeUser(Permission = Permission.MANAGE_REPORTING)]
        [Route("~/api/report/save")]
        public ActionResult SaveReport(int reportId, int dataSourceId, string[] colIds, Query[] queries, string name, OrderBy[] sorts)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Report report = new Report();
            report.ID = reportId;
            report.DataSource = dataSourceId;
            report.DisplayColumns = serializer.Serialize(colIds);
            report.Queries = serializer.Serialize(queries);
            report.Sorts = serializer.Serialize(sorts);
            report.Name = name;
            bool ret = false;
            if (reportId > 0)
            {
                ret = report.Update();
            }
            else
            {
                ret = report.Insert();
            }

            return Json(new { result = ret });
        }
        
        [AuthorizeUser(Permission = Permission.MANAGE_REPORTING)]
        [Route("~/api/report/delete")]
        public ActionResult DeleteReport(int reportId)
        {
            Report report = new Report();
            report.ID = reportId;
            return Json(new { result = report.Delete() });
        }
        #endregion
        [Route("~/api/report/list")]
        public ActionResult GetReportList(string searchFor = "", int page = 1, int pageSize = 15)
        {
            int totalRow = 0;
            Report report = new Report();
            List<object> data = new List<object>();
            var reports = report.Get(page, pageSize, "[Name]", string.Format("[Name] LIKE N'%{0}%'", searchFor.AntiSQLInjection()));
            if (reports != null)
            {
                totalRow = reports[0].TotalRows;

                foreach (var r in reports)
                {
                    data.Add(new { id = r.ID, name = r.Name });
                }

            }
            return Json(new { reports = new { data, pageIndex = page, pageSize, totalRow } });
        }
        [Route("~/api/report/data")]
        public ActionResult GetReportData(int reportId, int page = 1, int pageSize = 5)
        {

            Report report = new Report();
            report.ID = reportId;
            report.Get();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string[] cols = serializer.Deserialize<string[]>(report.DisplayColumns);
            Query[] queries = serializer.Deserialize<Query[]>(report.Queries);
            OrderBy[] sorts = serializer.Deserialize<OrderBy[]>(report.Sorts ?? "null");

            return Json(new { id = report.ID, name = report.Name, result = GetData(report.DataSource, cols, queries, sorts, page, pageSize) });
        }
        
        
    }
    [AuthorizeUser]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ApprovalApiController : Controller
    {
        #region manage approvals
        [AuthorizeUser(Permission = Permission.MANAGE_APPROVAL)]
        [Route("~/api/rule/detail")]
        public ActionResult GetRuleDetail(int ruleId)
        {
            ApprovalRule rule = new ApprovalRule();
            rule.ID = ruleId;
            rule.Get();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Action[] actions = serializer.Deserialize<Action[]>(rule.Actions);
            Query[] queries = serializer.Deserialize<Query[]>(rule.Queries);
            return Json(new {
                id = rule.ID,
                name = rule.Name,
                itemTypeId = rule.AppliedItemType,
                queries,
                actions,
                nextState = rule.NextState,
                queryState = rule.QueryState,
                permissions = rule.Permissions
            });
        }
        [AuthorizeUser(Permission = Permission.MANAGE_APPROVAL)]
        [Route("~/api/rule/list")]
        public ActionResult GetRuleList(string searchFor = "", int page = 1, int pageSize = 15)
        {
            int totalRow = 0;
            ApprovalRule rule = new ApprovalRule();
            List<object> data = new List<object>();
            var rules = rule.Get(page, pageSize, "[Name]", string.Format("[Name] LIKE N'%{0}%'", searchFor.AntiSQLInjection()));
            if (rules != null)
            {
                totalRow = rules[0].TotalRows;

                foreach (var r in rules)
                {
                    data.Add(new { id = r.ID, name = r.Name });
                }

            }
            return Json(new { rules = new { data, pageIndex = page, pageSize, totalRow } });
        }
        [AuthorizeUser(Permission = Permission.MANAGE_APPROVAL)]
        [Route("~/api/rule/delete")]
        public ActionResult DeleteRule(int ruleId)
        {
            ApprovalRule rule = new ApprovalRule();
            rule.ID = ruleId;
            return Json(new { result = rule.Delete() });
        }
        [AuthorizeUser(Permission = Permission.MANAGE_APPROVAL)]
        [Route("~/api/rule/save")]
        public ActionResult SaveRule(int ruleId, int itemTypeId, string permissions, string nextState, string queryState, Query[] queries, Action[] actions, string name)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ApprovalRule rule = new ApprovalRule();
            rule.ID = ruleId;
            rule.Permissions = permissions;
            rule.NextState = nextState;
            rule.QueryState = queryState;
            rule.AppliedItemType = itemTypeId;
            rule.Actions = serializer.Serialize(actions);
            rule.Queries = serializer.Serialize(queries);
            rule.Name = name;
            rule.CreatedBy = SessionManager.CurrentUser.ID;
            bool ret = false;
            if (ruleId > 0)
            {
                ret = rule.Update();
            }
            else
            {
                ret = rule.Insert();
            }

            return Json(new { result = ret });
        }
        [AuthorizeUser(Permission = Permission.MANAGE_APPROVAL)]
        [Route("~/api/rule/itemType")]
        public ActionResult GetRuleItems()
        {
            List<object> lst = new List<object>();
            lst.Add(new { text = "", value = "" });
            CacheManager.AllItemTypes.Where(t => t.ApprovalProcess).ToList().ForEach(t =>
            {
                lst.Add(new { text = t.Display, valueId = t.ID });
            });
            return Json(lst);
        }
        #endregion
        #region any user
        [Route("~/api/item/approvalList")]
        public ActionResult GetApprovalList(string type, int page = 1, int pageSize = 15)
        {
            ItemType itemType = CacheManager.AllItemTypes[type];
            List<ModelData> items = null;
            var rules = Common.GetApprovalRules(itemType.ID);
            foreach(var rule in rules)
            {
                items = Common.GetMatchedApprovalList(rules, 0, page, pageSize);
                if (items == null) return Json(new { });
                ModelEntity item = CacheManager.CreateModelEntity(itemType.ID);

                int totalRow = Common.ManipulateDataList(items, item.Properties, itemType.DisplayProperty);
                return Json(new { items = new { data = items, pageIndex = page, pageSize, totalRow } });
            }
            
            return Json(new {  });

        }
        [AllowAnonymous]
        [Route("~/api/approval/check")]
        public ActionResult CheckApprovalItem(int itemId)
        {
            Item item = new Item();
            item.ID = itemId;
            item.Get();
            ItemType itemType = CacheManager.AllItemTypes[item.Type];
            if (SessionManager.CheckItemPermission(itemType) && itemType.ApprovalProcess)
            {
                ItemApproval approval = new ItemApproval();
                approval.ItemId = itemId;
                var result = approval.GetAll("[CreatedTime] DESC", "[ItemId] = " + itemId, "JOIN [sys_Account] (NOLOCK) ON [sys_Account].ID = [sys_ItemApproval].[Actor]", "[Username] [Approver]");
                if (result == null)
                {
                    if (item.CreatedBy == SessionManager.CurrentUser.ID)
                    {
                        //submit
                        return Json(new { approval = new { show = true, submitter = true, submittable = true } });
                    }

                }
                else
                {
                    List<object> approvalHis = new List<object>();
                    foreach (var his in result)
                    {
                        approvalHis.Add(new { approver = his.Approver, state = his.State, comment = his.Comment, time = his.CreatedTime.ToString("yyyy-MMM-dd HH:mm") });
                    }
                    var rule = Common.GetMatchedApprovalRule(item.Type, itemId);
                    if (rule != null)
                    {
                        return Json(new { approval = new { show = true, submittable = true, submitter = false, histories = approvalHis } });
                    }
                    
                    return Json(new { approval = new { show = true, submittable = false, histories = approvalHis } });
                }
                
            }
            
            return Json(new { approval = new { show = false} });
        }
        [Route("~/api/approval/submit")]
        public ActionResult SubmitItem(int itemId, string comment)
        {
            Item item = new Item();
            item.ID = itemId;
            item.Get();
            ItemType itemType = CacheManager.AllItemTypes[item.Type];
            if (itemType.ApprovalProcess)
            {
                ItemApproval approval = new ItemApproval();
                approval.ItemId = itemId;
                var result = approval.Get("[ItemId] = " + itemId);
                if (!result && item.CreatedBy == SessionManager.CurrentUser.ID)
                {
                    
                    //submit
                    approval.Actor = SessionManager.CurrentUser.ID;
                    approval.State = ApprovalState.SUBMITTED;
                    approval.Comment = comment;
                    approval.CreatedTime = DateTime.UtcNow;
                    approval.Insert();
                    item.State = approval.State;
                    item.Update();

                    return Json(new { result = true });
                }
                else
                {
                    var rule = Common.GetMatchedApprovalRule(item.Type, itemId);
                    if (rule != null)
                    {
                        approval.Actor = SessionManager.CurrentUser.ID;
                        approval.State = rule.NextState;
                        approval.Comment = comment;
                        approval.CreatedTime = DateTime.UtcNow;
                        approval.Insert();
                        item.State = approval.State;
                        item.Update();
                        Action[] actions = new JavaScriptSerializer().Deserialize<Action[]>(rule.Actions);
                        if(actions!=null && actions.Length > 0)
                        {
                            ModelEntity itemDetail = CacheManager.CreateModelEntity(itemType.ID);
                            itemDetail["ID"] = itemId;
                            itemDetail.Get();
                            foreach (var act in actions) itemDetail[act.prop.Replace("[I].","").Trim(new char[] { '[', ']'})] = act.value;
                            itemDetail.Update();
                        }
                        
                        return Json(new { result = true });
                    }
                    
                }
                
            }

            return Json(new { result = false });
        }
        [Route("~/api/approval/reject")]
        public ActionResult DeclineItem(int itemId, string comment)
        {
            Item item = new Item();
            item.ID = itemId;
            item.Get();
            ItemType itemType = CacheManager.AllItemTypes[item.Type];
            if (itemType.ApprovalProcess)
            {
                ItemApproval approval = new ItemApproval();
                approval.ItemId = itemId;
                approval.Actor = SessionManager.CurrentUser.ID;
                approval.State = ApprovalState.REJECTED;
                approval.Comment = comment;
                approval.CreatedTime = DateTime.UtcNow;
                approval.Insert();
                item.State = approval.State;
                item.Update();
                return Json(new { result = true });


            }

            return Json(new { result = false });
        }
        #endregion
    }
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class AttachmentApiController : Controller
    {
        [Route("~/api/attachment/list")]
        public ActionResult GetAttachmentList(int itemId, int page = 1, int pageSize = 15)
        {
            if (!SessionManager.CheckItemPermission(itemId)) return Unauthorized();
            Attachment attachment = new Attachment();
            var result = attachment.Get(page, pageSize, "[CreatedTime] DESC", "[ItemID] = " + itemId, "JOIN [sys_Account] (NOLOCK) ON [sys_Account].[ID] = [sys_Attachment].[CreatedBy]", "[sys_Account].[Username] [CreatedUser]");
            if (result == null) return Json(new { });
            int totalRow = 0;
            List<object> items = new List<object>();
            string baseUrl = VirtualPathUtility.ToAbsolute("~/attachment/download/");
            foreach (var r in result)
            {
                if (totalRow == 0) totalRow = r.TotalRows; 
                items.Add(new
                {
                    fileId = r.ID,
                    fileName = r.FileName,
                    url = baseUrl + Encryptor.SimpleEncrypt(itemId.ToString()) +"/"+ Encryptor.SimpleEncrypt(r.ID.ToString()),
                    userName =  r.CreatedUser,
                    description =  r.Description,
                    time =  r.CreatedTime.ToString("yyyy-MM-dd HH:mm")
                });
            }
            return Json(new { attachments = new { data = items, pageIndex = page, pageSize, totalRow } });
        }
        [AuthorizeUser]
        [Route("~/api/attachment/upload")]
        public ActionResult UploadAttachment(int itemId, HttpPostedFileBase file, string description)
        {
            if (itemId > 0)
            {
                if (!SessionManager.CheckItemPermission(itemId, true)) return Unauthorized();
                Attachment attachment = new Attachment();
                attachment.ItemID = itemId;
                attachment.Description = description;
                attachment.FileName = file.FileName;
                attachment.CreatedBy = SessionManager.CurrentUser.ID;
                attachment.CreatedTime = DateTime.UtcNow;
                attachment.Insert();
                //file.ContentType
                
                file.SaveAs(Server.MapPath("~/App_Data/Attachments/f" + attachment.ID + ".dat"));

                return Json(new { result = true });
            }
            return Json(new { result = false });
        }
        [Route("~/attachment/download/{sItemId}/{sFileId}")]
        public ActionResult DownloadAttachment(string sItemId, string sFileId)
        {
            int itemId = int.Parse(Encryptor.SimpleDecrypt(sItemId));
            if (!SessionManager.CheckItemPermission(itemId)) return Unauthorized();
            int fileId = int.Parse(Encryptor.SimpleDecrypt(sFileId));
            Attachment attachment = new Attachment();
            attachment.ID = fileId;
            if (attachment.Get())
            {
                if (!SessionManager.CheckItemPermission(attachment.ItemID)) return Unauthorized();
                

                string filePath = Server.MapPath("~/App_Data/Attachments/f" + fileId + ".dat");
                if(System.IO.File.Exists(filePath)) return new FilePathResult(filePath, "application/octet-stream") { FileDownloadName = attachment.FileName };
            }
            return HttpNotFound("Attachment not found");
        }
        [AuthorizeUser]
        [Route("~/api/attachment/delete")]
        public ActionResult DeleteAttachment(int fileId)
        {

            

            Attachment attachment = new Attachment();
            attachment.ID = fileId;
            if (attachment.Get())
            {
                if (!SessionManager.CheckItemPermission(attachment.ItemID, true)) return Unauthorized();

                string filePath = Server.MapPath("~/App_Data/Attachments/f" + fileId + ".dat");
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch { }//no need to handle
                    
                }
                attachment.Delete();
                return Json(new { result = true });
            }
            return Json(new { result = false });
        }
    }
}