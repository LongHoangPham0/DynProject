using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueMoon.DynWeb.Models
{
    public class ModelData : DataItem
    {
        public int ID
        {
            get { return this.ContainsKey("ID") ? int.Parse(this["ID"].ToString()) : 0; }
            set { this["ID"] = value; }
        }
        public int Type
        {
            get { return this.ContainsKey("Type") ? int.Parse(this["Type"].ToString()) : 0; }
            set { this["Type"] = value; }
        }

    }
}