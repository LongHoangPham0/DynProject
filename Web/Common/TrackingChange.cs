using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueMoon.DynWeb.Common
{
    [EntityTable(false, "ItemId", "UpdatedBy", "UpdatedTime", TableName = "sys_TrackingChange")]
    public class TrackingChange : BaseEntity<TrackingChange>
    {
        public int ItemId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}