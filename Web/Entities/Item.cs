using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable(true, "ID",TableName = "sys_Item")]
    public class Item : BaseEntity<Item>
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public string State { get; set; }

        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public int UpdatedBy { get; set; }

        
    }
}