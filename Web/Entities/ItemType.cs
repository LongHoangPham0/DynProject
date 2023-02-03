using BlueMoon.Business;
using BlueMoon.MVC.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable(true, "ID", TableName = "sys_ItemType")]
    public class ItemType : BaseEntity<ItemType>
    {
        [Input(ControlType.Hidden)]
        public int ID { get; set; }
        [Input(Validation = "[{\"rule\": \"required\", \"msg\": \"Name is required\"},{\"rule\":\"regExp\",\"msg\": \"At least 2 characters, start with A-Z. Only A-Z & 0-9 are accepted\", \"options\":{\"pattern\":\"[a-zA-Z][a-zA-Z0-9]+\"}}]")]
        public string Name { get; set; }
        [Input("Display attribute", Validation = "[{\"rule\": \"required\", \"msg\": \"Display attribute is required\"}]")]
        public string DisplayProperty { get; set; }
        [Input]
        public string Display { get; set; }
        [Input("Access permission")]
        public string Permission { get; set; }
        [Input("Mod permission")]
        public string ModPermission { get; set; }
        [Input("Position")]
        public int SortOrder { get; set; }
        [Input("Type", ControlType.Dropdown, "TypeOfItemList")]
        public int TypeOfItem { get; set; }
        //[Input("Child item")]
        //public bool ChildOnly { get; set; }
        //[Input("Ref item")]
        //public bool RefItem { get; set; }

        [Input("List by creator")]
        public bool ListByCreator { get; set; }
        [Input("Comment")]
        public bool AllowComment { get; set; }
        [Input("Reporting")]
        public bool AllowReport { get; set; }
        [Input("Approval")]
        public bool ApprovalProcess { get; set; }
        [Input("Attachment")]
        public bool AllowFileAttachment { get; set; }
        [Input("Tracking")]
        public bool TrackingChange { get; set; }

        public List<ItemType> GetListItemType()
        {
            return GetAll("[SortOrder]");
        }

        public void Delete(int itemTypeID)
        {
            ObjectParameter objectParameter = new ObjectParameter();
            objectParameter.Add("ItemTypeID", itemTypeID);
            Db.ExecuteSpa("sp_DeleteItemType", objectParameter);
        }
        public List<ItemTypeColumn> GetAllCols(int itemTypeID, byte mode = 0)
        {
            ObjectParameter objectParameter = new ObjectParameter();
            objectParameter.Add("ItemTypeID", itemTypeID);
            if (mode != 0) objectParameter.Add("ViewMode", mode);
            return Db.ExecuteSpa<ItemTypeColumn>("sp_GetAvailableCols", objectParameter);
        }

        [IgnoreProperty]
        public List<SelectListItem> TypeOfItemList
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                Array vals = Enum.GetValues(typeof(TypeOfItem));
                string[] txts = Enum.GetNames(typeof(TypeOfItem));
                for(int i = 0; i < vals.Length; i++)
                {
                    items.Add(new SelectListItem() { Value = (int)vals.GetValue(i) + "", Text = txts[i] });
                }
                return items;
            }
        }
    }
    public class ItemTypeColumn
    {
        public string ColName { get; set; }
        public string Alias { get; set; }
        public string Label { get; set; }
    }
    public enum TypeOfItem
    {
        Normal = 0,
        ChildOnly = 1,
        RefItem = 2
    }
}