using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Common;
using BlueMoon.MVC.Controls;
using System.Web.Mvc;

namespace BlueMoon.DynWeb.Entities
{
    [EntityTable(true, "ID", TableName = "sys_Account")]
    public class Account : BaseEntity<Account>
    {
        const char ATT_SAPERATOR = '|';
        const char SAPERATOR = ',';
        [Input(ControlType.Hidden)]
        public int ID { get; set; }
        [Input(ControlType.Auto)]
        public string Username { get; set; }
        [IgnoreProperty]
        [Input("Reset password", ControlType.Auto)]
        public bool ResetPwd { get; set; }
        [Input(ControlType.Textbox)]
        public string Password { get; set; }
        [Input("Role", ControlType.Dropdown, "Roles")]
        public int RoleID { get; set; }

        [Input("Linking items",ControlType.Textbox)]
        public string LinkedIDs { get; set; }
        [IgnoreProperty]
        public List<Permission> Permission { get; set; }

        public bool HasPermission(params string[] names)
        {
            foreach (var name in names)
            {
                if (string.IsNullOrEmpty(name)) return true;
                if (Permission != null)
                {
                    if (Permission.Exists(m => m.Name.ToLower() == name.ToLower()))
                    {
                        return true;
                    }
                }

            }
            return false;
        }
        public bool CheckAccount(string username, string password)
        {
            return Get(string.Format("[Username] = N'{0}' AND [Password] = N'{1}'", username.AntiSQLInjection(), password.AntiSQLInjection()));
        }
        public bool GetAccount(string username)
        {
            return Get(string.Format("[Username]='{0}'", username.AntiSQLInjection()));
        }
        public List<Account> GetListAccount()
        {
            return GetAll("[Username] ASC");
        }

        Dictionary<int, int> _typeIds = null;
        [IgnoreProperty]
        public ItemType[] LinkedItems {
            get
            {
                if (!string.IsNullOrEmpty(LinkedIDs))
                {
                    if (_typeIds == null)
                    {
                        _typeIds = new Dictionary<int, int>();
                        var ids = LinkedIDs.Split(SAPERATOR);
                        foreach (var idt in ids)
                        {
                            var id = idt.Split('|');
                            Item item = new Item();
                            item.ID = int.Parse(id[0]);
                            if (item.Get())
                            {
                                _typeIds.Add(item.Type, item.ID);
                            }
                            
                        }
                    }
                    
                    return CacheManager.AllItemTypes.Where(p => _typeIds.ContainsKey(p.ID)).ToArray();
                }
                return null;
            }
        }
        public string RemoveUIInfoFromLinkedIds()
        {
            string result = null;
            if (!string.IsNullOrEmpty(LinkedIDs))
            {
                var ids = LinkedIDs.Split(SAPERATOR);
                result = string.Join(SAPERATOR.ToString(), ids.Select(p=>p.Split(ATT_SAPERATOR)[0]));
            }

            return result;
        }
        public string GetLinkedIdsForUIControl()
        {
            string result = null;
            if (!string.IsNullOrEmpty(LinkedIDs))
            {
                var ids = LinkedIDs.Split(SAPERATOR);
                foreach (var id in ids)
                {
                    Item item = new Item();
                    item.ID = int.Parse(id);
                    
                    if (item.Get())
                    {
                        var itemType = CacheManager.AllItemTypes[item.Type];
                        var itemDetail = CacheManager.CreateModelEntity(item.Type);
                        itemDetail["ID"] = item.ID;
                        if (itemDetail.Get())
                        {
                            result += item.ID.ToString().Encrypt() + ATT_SAPERATOR + itemType.Display + " - " + itemDetail[itemType.DisplayProperty] + SAPERATOR;
                        }
                    }

                }
                result = result.Trim(SAPERATOR);
            }
            
            return result;
        }
        public int GetLinkedItemId(int typeId)
        {
            if (_typeIds?.ContainsKey(typeId) ?? false) return _typeIds[typeId];
            else return 0;
        }
        public bool IsLinkedItem(int itemId)
        {
            return _typeIds?.ContainsValue(itemId) ?? false;
        }
        [IgnoreProperty]
        public List<SelectListItem> Roles
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                Role role = new Role();
                var vals = role.GetAll("[Name] ASC");
                foreach (var v in vals)
                {
                    items.Add(new SelectListItem() { Value = v.ID.ToString(), Text = v.Name });
                }
                return items;
            }
        }
        public class LinkedItem
        {
            public ItemType ItemType { get; set; }
            public int Value { get; set; }
        }
    }
}