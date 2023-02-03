using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Models;
using BlueMoon.MVC.Controls;
using BlueMoon.DynWeb.Entities;

namespace BlueMoon.DynWeb.Common
{
    public static class Utilities
    {
        public static string AntiSQLInjection(this string sql)
        {
            if (string.IsNullOrEmpty(sql)) return sql;
            else return sql.Replace("'", "''").Replace("--", "");
        }
        public static List<ModelData> ToModelData(this List<DataItem> list, int type)
        {
            if (list != null && list.Count > 0)
            {
                List<ModelData> result = new List<ModelData>();
                foreach (DataItem o in list)
                {
                    ModelData data = new ModelData();
                    data.Copy(o);
                    data.Type = type;
                    result.Add(data);
                }
                return result;
            }
            return null;
        }
        public static string ToDataSourceName(this string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (name.StartsWith("[")) return null;
            return name.Split(':')[0];
        }
        public static PropertyDefinition CloneAsPropertyDefinition(this ItemTypeProperty prop)
        {
            return new PropertyDefinition(prop.PropertyName, prop.ToModelDataType(), prop.AllowShowGrid, (ControlType)Enum.Parse(typeof(ControlType), prop.InputControl), prop.LabelText, prop.DataSource, prop.Validation, prop.ReadOnly, prop.OnValueChanged, prop.AllowSearch, prop.DisplayFormat);
        }
    }
}