using BlueMoon.Business;
using BlueMoon.DynWeb.Entities;
using BlueMoon.MVC.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace BlueMoon.DynWeb.Common
{
    public class DataItemConverter : BlueMoon.MVC.IDataItemConverter
    {
        public object CastValue(int type, string propertyName, string attemptedValue)
        {
            if (propertyName == "ID") return int.Parse(attemptedValue);
            ModelDefinition defi = CacheManager.GetModelDef(type);
            PropertyDefinition prop = defi[propertyName];

            if (prop != null)
            {
                if (!string.IsNullOrEmpty(prop.Validation))
                {
                    //try to parse json
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    object[] lstRuleVal = (object[])serializer.DeserializeObject(prop.Validation);//NhaDH add
                    string msg = Validation.Validate(attemptedValue, lstRuleVal);
                    if (msg != null) throw new Exception(propertyName + " has invalid value", new Exception(msg));
                }
                switch (prop.DataType)
                {
                    case DataType.Bool:
                        if (attemptedValue.IndexOf(',') > 0) attemptedValue = attemptedValue.Split(',')[0];
                        return bool.Parse(attemptedValue.ToLower());
                    case DataType.Int:
                        return string.IsNullOrEmpty(attemptedValue) ? 0 : int.Parse(attemptedValue);
                    case DataType.String:
                        break;
                }
            }
            
            return attemptedValue;
        }
        public void Validate(string type, DataItem data)
        {
            ItemType itemType = CacheManager.AllItemTypes[type];
            ModelDefinition defi = CacheManager.GetModelDef(itemType.ID);
            foreach (var propp in data)
            {
                PropertyDefinition prop = defi[propp.Key];

                if (prop != null)
                {
                    if (!string.IsNullOrEmpty(prop.Validation))
                    {
                        //try to parse json
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        object[] rules = (object[])serializer.DeserializeObject(prop.Validation);//NhaDH add
                        string msg = Validation.Validate(propp.Value?.ToString(), rules);
                        if (msg != null) throw new Exception(propp.Key + " has invalid value", new Exception(msg));
                    }
                }
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //object[] lstRuleVal = (object[])serializer.DeserializeObject(prop.Validation);//NhaDH add
                //string msg = Validation.Validate(attemptedValue, lstRuleVal);
                //if (msg != null) throw new Exception(propertyName + " has invalid value", new Exception(msg));
            }
            
        }
        public void Validate(string propName, string attemptedValue, object[] rules)
        {
            string msg = Validation.Validate(attemptedValue, rules);
            if(msg != null) throw new Exception(propName + " has invalid value", new Exception(msg));
        }
    }
}