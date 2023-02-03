using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable(true,"ID", TableName = "sys_Permission")]
    public class Permission: BaseEntity<Permission>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Permission> GetPermissionOfRole(int RoleID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("RoleID", RoleID);
            return Db.ExecuteSpa<Permission>("sp_GetPermissionOfRole", op);
        }
        public List<Permission> GetListPermission()
        {
            return GetAll("[Name] ASC");
        }
        public void Delete(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", ID);
            Db.ExecuteSpa("sp_DeletePermission", op);
        }

        
        public const string MANAGE_SYSTEM = "sys_manage_system";
        public const string MANAGE_ITEMS = "sys_manage_items";
        public const string MANAGE_BUSINESS = "sys_manage_business";
        public const string MANAGE_REPORTING = "sys_manage_reporting";
        public const string MANAGE_APPROVAL = "sys_manage_approval";
        public const string ACCESS_REPORTING = "sys_access_reporting";
    }
}