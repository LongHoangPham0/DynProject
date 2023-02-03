using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable("RoleID","PermissionID", TableName = "sys_RolePermission")]
    public class RolePermission : BaseEntity<RolePermission>
    {
        public int RoleID { get; set; }
        public int PermissionID { get; set; }
    }
}