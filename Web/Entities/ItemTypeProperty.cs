using BlueMoon.Business;
using BlueMoon.MVC.Controls;
using BlueMoon.DynWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable(true, "ID", TableName = "sys_ItemTypeProperty")]
    public class ItemTypeProperty : BaseEntity<ItemTypeProperty>
    {
        [Input(ControlType.Hidden)]
        public int ID { get; set; }
        [Input("Attribute name",Validation = "[{\"rule\": \"required\", \"msg\": \"Attribute name is required\"},{\"rule\":\"regExp\",\"msg\": \"At least 2 characters, start with A-Z. Only A-Z & 0-9 are accepted\", \"options\":{\"pattern\":\"[a-zA-Z][a-zA-Z0-9]+\"}}]")]
        public string PropertyName { get; set; }
        [Input("Data type", ControlType.Dropdown, "DataTypeList")]
        public string DataType { get; set; }
        [Input(ControlType.Hidden)]
        public int ItemType { get; set; }
        [Input("Label", Validation = "[{\"rule\": \"required\", \"msg\": \"Label is required\"}]")]
        public string LabelText { get; set; }
        [Input("Input control", ControlType.Dropdown, "InputControlList")]
        public string InputControl { get; set; }
       
        [Input("Data source (item_name; item_name:filter_by_prop;[{\"text\":string, \"valueId\":string}])", ControlType.TextArea)]
        public string DataSource { get; set; }
        [Input("Display format ({0:#,##0}, {0:dd/MM/yyyy}, ...)")]
        public string DisplayFormat { get; set; }
        [Input("Position", Validation = "[{\"rule\": \"required\", \"msg\": \"Position is required\"}]")]
        public int SortOrder { get; set; }
        [Input("",ControlType.TextArea)]
        public string Validation { get; set; }
        [Input("", ControlType.TextArea)]
        public string OnValueChanged { get; set; }

        [Input("Allow search")]
        public bool AllowSearch { get; set; }
        [Input("Show in grid")]
        public bool AllowShowGrid { get; set; }
        [Input("Read only")]
        public bool ReadOnly { get; set; }
        public List<ItemTypeProperty> GetPropertiesOfItem(int itemType)
        {
            ObjectParameter objectParameter = new ObjectParameter();
            objectParameter.Add("ItemType", itemType);
            return Db.ExecuteSpa<ItemTypeProperty>("sp_GetPropertiesOfItem", objectParameter);
        }
        public void Delete(int ID)
        {
            ObjectParameter objectParameter = new ObjectParameter();
            objectParameter.Add("ID", ID);
            Db.ExecuteSpa("sp_DeleteItemTypeProperty", objectParameter);
        }
        public DataType ToModelDataType()
        {
            return (DataType)Enum.Parse(typeof(DataType), DataType, true);
        }
        [IgnoreProperty]
        public List<SelectListItem> DataTypeList
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                string[] vals = Enum.GetNames(typeof(DataType2));
                foreach (var v in vals)
                {
                    items.Add(new SelectListItem() { Value = v, Text = v });
                }
                return items;
            }
        }

        [IgnoreProperty]
        public List<SelectListItem> InputControlList
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                string[] vals = Enum.GetNames(typeof(ControlType2));
                foreach (var v in vals)
                {
                    items.Add(new SelectListItem() { Value = v, Text = v });
                }
                return items;
            }
        }

        enum ControlType2
        {
            Auto,
            Hidden,
            Textbox,
            Checkbox,
            Dropdown,
            DatePicker,
            TextArea,
            RadioList,
            CheckboxList
        }
        enum DataType2
        {
            String,
            Int,
            DateTime,
            Bool,
            Decimal,
            Text
        }
    }
}