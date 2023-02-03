using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Entities;
namespace BlueMoon.DynWeb.Common
{
    public static class SessionManager
    {
        public static Account CurrentUser 
        {
            get 
            {
                return (Account)HttpContext.Current.Session["CurrentUser"];
            }
            set 
            {
                HttpContext.Current.Session["CurrentUser"] = value;
            }
        }
        public static bool CheckItemPermission(ItemType item, bool mod = false)
        {
            if (item.Permission?.ToLower() == "anonymous") return true;
            else if (CurrentUser == null) return false;
            else if (item.TypeOfItem == (int)TypeOfItem.RefItem) return CurrentUser.HasPermission(Permission.MANAGE_BUSINESS);
            else return CurrentUser.HasPermission(mod ? new string[] { item.ModPermission } : new string[] { item.Permission });
        }
        //should cache itemId & itemType
        public static bool CheckItemPermission(Item item, bool mod = false)
        {
            if (SessionManager.CurrentUser.IsLinkedItem(item.ID)) return true;
            return CheckItemPermission(CacheManager.AllItemTypes[item.Type], mod);
        }
        public static bool CheckItemPermission(int itemId, bool mod = false)
        {
            if (SessionManager.CurrentUser.IsLinkedItem(itemId)) return true;
            Item item = null;
            if (s_ItemCached.ContainsKey(itemId)) item = s_ItemCached[itemId];
            else
            {
                item = new Item();
                item.ID = itemId;
                item.Get();
                s_ItemCached[itemId] = item;
            }
            return CheckItemPermission(item, mod);
        }
        private static Dictionary<int, Item> s_ItemCached = new Dictionary<int, Item>();
        
    }
}