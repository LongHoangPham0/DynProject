using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable(true,"ID", TableName = "sys_Role")]
    public class Role: BaseEntity<Role>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RoleCode { get; set; }
        public bool GetByRoleCode(string code)
        {
            return Get("[RoleCode] = '" + code.AntiSQLInjection() + "'");
        }
        public List<Role> GetListRole()
        {
            return GetAll("[Name] ASC");
        }
        public void DeleteRolePermission(int RoleID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", RoleID);
            Db.ExecuteSpa("sp_DeleteRolePermission", op);
        }
        public void Delete(int ID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ID", ID);
            Db.ExecuteSpa("sp_DeleteRole", op);
        }
    }
}