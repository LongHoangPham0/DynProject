using BlueMoon.MVC.Controls;
using BlueMoon.Business;
using BlueMoon.DynWeb.Common;
using BlueMoon.DynWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlueMoon.DynWeb.Entities;
using System.Text.RegularExpressions;

namespace BlueMoon.DynWeb.Controllers
{
    [AuthorizeUser(Permission = Permission.MANAGE_ITEMS)]
    public class ItemController : Controller
    {
        [Route("~/system/itemtype/list")]
        public ActionResult ListItemType(SystemModel model)
        {
            ItemType itemType = new ItemType();
            List<ItemType> lstItemType = new List<ItemType>();
            lstItemType = itemType.GetListItemType();
            model.ChildItemSelected = new List<ItemTypeRelation>();
            model.ReturnUrl = Request.RawUrl;
            if (lstItemType != null) model.ListItemType = lstItemType;
            return View(model);
        }
        [Route("~/system/itemtype/manage")]
        public ActionResult ManageItemType(SystemModel model)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                if (model.ItemType == null) model.ItemType = new ItemType();
                //ItemType itemType = new ItemType();
                model.ListItemType = model.ItemType.GetListItemType();

                if (model.ItemType.ID > 0)
                {
                    //itemType.ID = model.ItemType.ID;
                    model.ItemType.Get();
                    //model.ItemType = itemType;

                    List<ItemTypeProperty> lstProperty = new List<ItemTypeProperty>();
                    ItemTypeProperty itemProperty = new ItemTypeProperty();
                    lstProperty = itemProperty.GetPropertiesOfItem(model.ItemType.ID);
                    model.ListProperty = lstProperty;

                    model.ItemTypeProperty = new ItemTypeProperty();
                    model.ItemTypeProperty.ItemType = model.ItemType.ID;

                    model.ReturnUrl = Request.RawUrl;
                    model.ChildItemSelected = new List<ItemTypeRelation>();
                    ItemTypeRelation itemChild = new ItemTypeRelation();
                    model.ChildItemSelected = itemChild.GetListChildItem(model.ItemType.ID);
                }
                else
                {
                    model.ChildItemSelected = new List<ItemTypeRelation>();
                }
            }
            else
            {
                if (model.ItemType.ID > 0)
                {
                    ItemType itemType = new ItemType();
                    itemType.ID = model.ItemType.ID;
                    itemType.Get();
                    model.ItemType.Name = itemType.Name;//copy to ensure Name is not changed
                    model.ItemType.Update();
                    ItemTypeRelation itemChild = new ItemTypeRelation();
                    itemChild.Delete(model.ItemType.ID);

                    foreach (var item in model.ChildItemSelected)
                    {
                        if (item.ChildTypeID != 0)
                        {
                            itemChild.ParentTypeID = model.ItemType.ID;
                            itemChild.ChildTypeID = item.ChildTypeID;
                            itemChild.Type = item.Type;
                            itemChild.Alias = item.Alias;
                            itemChild.SortOrder = item.SortOrder;
                            itemChild.Insert();
                        }
                    }
                    
                    CacheManager.ClearItemTypeCache(model.ItemType.ID);
                }
                else
                {
                    model.ItemType.Insert();

                    if (model.ChildItemSelected != null)
                    {
                        foreach (var item in model.ChildItemSelected)
                        {
                            if (item.ChildTypeID != 0)
                            {
                                ItemTypeRelation itemChild = new ItemTypeRelation();
                                itemChild.ParentTypeID = model.ItemType.ID;
                                itemChild.ChildTypeID = item.ChildTypeID;
                                itemChild.Type = item.Type;
                                itemChild.Insert();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(model.ItemType.DisplayProperty))
                    {
                        ItemTypeProperty itemTypeProperty = new ItemTypeProperty();
                        itemTypeProperty.PropertyName = model.ItemType.DisplayProperty;
                        itemTypeProperty.DataType = "String";
                        itemTypeProperty.ItemType = model.ItemType.ID;
                        itemTypeProperty.LabelText = model.ItemType.DisplayProperty;
                        itemTypeProperty.InputControl = "Textbox";
                        itemTypeProperty.Validation = "[{rule: 'required', msg: '"+ itemTypeProperty.LabelText + " is required'}]";
                        itemTypeProperty.DataSource = "";
                        itemTypeProperty.DisplayFormat = "";
                        itemTypeProperty.AllowShowGrid = true;
                        itemTypeProperty.OnValueChanged = "";
                        itemTypeProperty.Insert();
                    }
                    
                }
                CacheManager.ClearListItemTypeCache();
                return RedirectToAction("ListItemType");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteItemType(int ID)
        {
            ItemType itemType = new ItemType();
            itemType.Delete(ID);
            CacheManager.ClearListItemTypeCache();
            return RedirectToAction("ListItemType");
        }
        [HttpPost]
        public ActionResult DeleteProperty(int ID)
        {
            ItemTypeProperty itemProperty = new ItemTypeProperty();
            itemProperty.ID = ID;
            itemProperty.Get();
            itemProperty.Delete(ID);
            CacheManager.ClearItemTypeCache(itemProperty.ItemType);
            return Redirect(Url.Action("ManageItemType") + "?itemtype.id=" + itemProperty.ItemType);
        }
        public ActionResult ManageItemProperty(SystemModel model)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                if (model.ItemTypeProperty == null) model.ItemTypeProperty = new ItemTypeProperty();
                if (model.ItemTypeProperty.ID > 0)
                {
                    model.ItemTypeProperty.Get();
                    //manipulate ids, encrypt
                    var s = model.ItemTypeProperty.OnValueChanged;
                    if(!string.IsNullOrEmpty(s)) model.ItemTypeProperty.OnValueChanged = Regex.Replace(s, @"fields:\[(?<v>.*?)\]", m =>
                    {
                        string v = m.Groups["v"].Value;
                        if (string.IsNullOrEmpty(v)) return m.Value;
                        var vs = v.Split(',');
                        for (int i = 0; i < vs.Length; i++)
                        {
                            vs[i] = "'" + vs[i].Trim('\'').Encrypt() + "'";
                        }
                        //string[]
                        return string.Format("fields:[{0}]", string.Join(",", vs));
                    });

                }
                else
                {
                    model.ItemTypeProperty.ItemType = model.ItemType.ID;
                }
            }
            else
            {
                //manipulate post ids, decrypt
                model.ItemTypeProperty.OnValueChanged = model.ItemTypeProperty.OnValueChanged.DecryptAll();
                if (model.ItemTypeProperty.ID > 0)
                {
                    ItemTypeProperty itemProperty = new ItemTypeProperty();
                    itemProperty.ID = model.ItemTypeProperty.ID;
                    itemProperty.Get();

                    model.ItemTypeProperty.PropertyName = itemProperty.PropertyName;
                    model.ItemTypeProperty.Update();
                }
                else
                {
                    model.ItemTypeProperty.Insert();
                }
                CacheManager.ClearItemTypeCache(model.ItemTypeProperty.ItemType);
                model.Close = true;
            }
            return View(model);
        }

        [Route("~/system/itemtype/properties")]
        public ActionResult GetItemProperties(int typeId)
        {
            var props = CacheManager.CreateModelEntity(typeId).Properties;
            List<DataItem> items = new List<DataItem>();
            props.ForEach(p =>
            {
                if(p.Name!="ID" )
                { 
                    DataItem item = new DataItem();
                    item["valueId"] = item["text"] = p.Name;
                    items.Add(item);
                }
                
            });
            return Json(items);
        }
    }
}