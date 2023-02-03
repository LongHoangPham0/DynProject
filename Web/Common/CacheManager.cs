using BlueMoon.Business;
using BlueMoon.MVC.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlueMoon.DynWeb.Entities;
namespace BlueMoon.DynWeb.Common
{
    
    public static class CacheManager
    {
        public static void ClearItemTypeCache(int id)
        {
            HttpContext.Current.Cache.Remove(ENTITY_CACHE_KEY + id);
            DofGenerator.DeleteFieldDefs();
            DofGenerator.DeleteViewDefs();
        }
        public static void ClearListItemTypeCache()
        {
            HttpContext.Current.Cache.Remove(ITEM_TYPE_KEY);
        }
        public static ModelDefinition GetModelDef(int type)
        {
            EnsureCache(type);
            return ((ModelEntity)HttpContext.Current.Cache[ENTITY_CACHE_KEY + type]).Properties;
        }
        public static ModelEntity CreateModelEntity(int type)
        {
            EnsureCache(type);
            return ((ModelEntity)HttpContext.Current.Cache[ENTITY_CACHE_KEY + type]).Clone();
        }
        public static ItemTypeCollection AllItemTypes
        {
            get
            {
                if (HttpContext.Current.Cache[ITEM_TYPE_KEY] == null)
                {
                    ItemType type = new ItemType();
                    List<ItemType> list = type.GetListItemType();
                    if (list != null) HttpContext.Current.Cache[ITEM_TYPE_KEY] = new ItemTypeCollection(list);
                }
                return (ItemTypeCollection)HttpContext.Current.Cache[ITEM_TYPE_KEY];
            }
        }
        const string ITEM_TYPE_KEY = "all_types";
        const string ENTITY_CACHE_KEY = "model_entity_";
        static void EnsureCache(int id)
        {
            string entityKey = ENTITY_CACHE_KEY + id;
            if (HttpContext.Current.Cache[entityKey] == null)
            {
                ItemTypeProperty objectStructure = new ItemTypeProperty();
                List<ItemTypeProperty> properties = objectStructure.GetPropertiesOfItem(id);
                
                if (properties != null)
                {
                    ModelDefinition defiEntity = new ModelDefinition(AllItemTypes[id].Name, "ID", AllItemTypes[id].DisplayProperty);
                    defiEntity.Add(new PropertyDefinition("ID", DataType.Int, false, ControlType.Hidden));

                    foreach (var p in properties) defiEntity.Add(p.CloneAsPropertyDefinition());
                    ModelEntity item = new ModelEntity(id, defiEntity);
                    item.CreateTable();
                    HttpContext.Current.Cache[entityKey] = item;
                }
            }
        }
                
        public class ItemTypeCollection : List<ItemType>
        {
            public ItemTypeCollection(IEnumerable<ItemType> collection) : base(collection) { }
            public ItemType this[string key]
            {
                get
                {
                    return this.Find(o => o.Name.ToLower() == key.ToLower());
                }
            }
            public new ItemType this[int id]
            {
                get
                {
                    return this.Find(o => o.ID == id);
                }
            }
        }
    }
}