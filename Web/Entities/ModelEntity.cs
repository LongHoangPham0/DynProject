using BlueMoon.Business;
using BlueMoon.MVC.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BlueMoon.DynWeb.Common;
using BlueMoon.DynWeb.Models;

namespace BlueMoon.DynWeb.Entities
{
    public class ModelEntity : DataItemEntity
    {
        public int Type { get; set; }
        public ModelEntity(int typeId, ModelDefinition defi)
            : base(defi)
        { Type = typeId; }
        public void EnsureDataSource()
        {
            foreach (PropertyDefinition prop in Properties)
            {
                if (!string.IsNullOrEmpty(prop.DataSourceField))
                {
                    ItemType type = CacheManager.AllItemTypes[prop.DataSourceField];
                    ModelEntity entity = CacheManager.CreateModelEntity(type.ID);
                    List<ModelData> list = entity.GetChildList(1000);
                    List<SelectListItem> ds = new List<SelectListItem>();
                    foreach (ModelData item in list)
                    {
                        
                        ds.Add(new SelectListItem() { Text = (string)item[type.DisplayProperty], Value = item["ID"].ToString() });
                    }
                    this[prop.Name + ":DataSource"] = ds;
                }
            }
        }
        public List<ModelData> GetChildList(int pageSize, int pageIndex = 1, int parentId = 0)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("PageIndex", pageIndex);
            op.Add("PageSize", pageSize);
            op.Add("Type", Type);
            op.Add("ParentId", parentId);
            List<DataItem> list = Db.ExecuteSpa("sp_GetItems", op);
            return list.ToModelData(Type);
        }
        

        public List<ModelData> GetApprovalList(string[] queryStates, Query[][] queriesList, int itemId, int pageSize, int pageIndex = 1)
        {
            StringBuilder queryFilter = new StringBuilder();
            for(int i = 0; i < queryStates.Length; i++)
            {
                if (i == 0) queryFilter.Append("("); 
                else queryFilter.Append(" OR (");
                queryFilter.Append(string.Format("[sys_Item].[State] = '{0}'", queryStates[i].AntiSQLInjection()));
                if (queriesList[i] != null && queriesList[i].Length > 0) foreach (var q in queriesList[i])
                    {
                        queryFilter.Append(" AND " + q);
                    }
                queryFilter.Append(")");
            }
            
            ObjectParameter op = new ObjectParameter();
            op.Add("PageIndex", pageIndex);
            op.Add("PageSize", pageSize);
            op.Add("Type", Type);
            op.Add("QueryFilter", queryFilter.ToString());
            op.Add("ItemId", itemId);
            List<DataItem> list = Db.ExecuteSpa("sp_GetApprovalList", op);
            return list.ToModelData(Type);
        }
        public List<ModelData> SelectChildItems(int parentId, string filter, int otherParentId, int pageSize, int pageIndex = 1)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("PageIndex", pageIndex);
            op.Add("PageSize", pageSize);
            op.Add("ParentId", parentId);
            op.Add("Filter", filter);
            op.Add("OtherParentId", otherParentId);
            op.Add("Type", Type);
            List<DataItem> list = Db.ExecuteSpa("sp_SelectChildItems", op);
            return list.ToModelData(Type);
        }
        
        public List<ModelData> SearchItems(string queryFilter, int pageSize, int pageIndex = 1)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("PageIndex", pageIndex);
            op.Add("PageSize", pageSize);
            op.Add("Type", Type);
            op.Add("QueryFilter", queryFilter);
            List<DataItem> list = Db.ExecuteSpa("sp_SearchItems", op);
            return list.ToModelData(Type);
        }
        public List<ModelData> GetRefItems(string filter, int parentId, int itemId, int pageSize, int pageIndex = 1)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("PageIndex", pageIndex);
            op.Add("PageSize", pageSize);
            op.Add("Type", Type);
            op.Add("Filter", filter);
            op.Add("ItemId", itemId);
            op.Add("ParentId", parentId);
            List<DataItem> list = Db.ExecuteSpa("sp_GetRefItems", op);
            return list.ToModelData(Type);
        }
        public bool Delete(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", ID);
            return Db.ExecuteNonQuerySpa("sp_DeleteItem", op) > 0;

        }
        public new ModelEntity Clone()
        {
            return new ModelEntity(Type, Properties);
        }

        public List<dynamic> Differ(ModelEntity newEntity)
        {
            List<dynamic> dif = new List<dynamic>();
            foreach (var p in Properties)
            {
                if (!this[p.Name].Equals(newEntity[p.Name]) && this[p.Name].ToString() != newEntity[p.Name].ToString())
                {
                    dif.Add(new { Field = p.Name, OldValue = this[p.Name], NewValue = newEntity[p.Name] });
                }
            }
            if (dif.Count > 0) return dif;
            else return null;
        }

        #region not in used
        public List<ModelData> GetParentList(int pageSize, int pageIndex = 1, int childId = 0)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("PageIndex", pageIndex);
            op.Add("PageSize", pageSize);
            op.Add("Type", Type);
            op.Add("ChildId", childId);
            List<DataItem> list = Db.ExecuteSpa("sp_GetParentItems", op);
            return list.ToModelData(Type);
        }
        public List<int> GetIndirectParents(int parentId, int type)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("TypeID", type);
            op.Add("ParentID", parentId);
            return Db.ExecuteSpa<int>("sp_GetIndirectParents", op);
        }

        #endregion
    }
}