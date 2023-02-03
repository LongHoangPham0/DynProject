using BlueMoon.MVC.Controls;
using BlueMoon.DynWeb.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using BlueMoon.DynWeb.Entities;
namespace BlueMoon.DynWeb.Models
{
    public class SystemModel
    {
        public ItemType ItemType { get; set; }
        public IEnumerable<ItemType> ListItemType { get; set; }
        public ItemTypeProperty ItemTypeProperty { get; set; }
        public IEnumerable<ItemTypeProperty> ListProperty { get; set; }
        public List<ItemTypeRelation> ChildItemSelected { get; set; }
        public string ReturnUrl { get; set; }
        public bool Close { get; set; }
        public Role Role { get; set; }
        public List<Role> ListRole { get; set; }
        public Permission Permission { get; set; }
        public List<int> PermissionSelected { get; set; }
        public List<Permission> ListPermission { get; set; }
        public Account Account { get; set; }
        public List<Account> ListAccount { get; set; }
    }
}