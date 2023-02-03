using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable("IDChild", "IDParent", TableName = "sys_ItemRelation")]
    public class ItemRelation : BaseEntity<ItemRelation>
    {
        public int IDChild { get; set; }
        public int IDParent { get; set; }
        public string Description { get; set; }
    }
}