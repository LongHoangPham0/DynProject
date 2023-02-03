using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable(true, "ID", TableName = "sys_Attachment")]
    public class Attachment: BaseEntity<Attachment>
    {
        public int ID { get; set; }

        public int ItemID { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }

        [IgnoreProperty]
        public string CreatedUser { get; set; }
    }
}