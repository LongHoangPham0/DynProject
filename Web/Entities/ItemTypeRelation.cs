using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable("ParentTypeID", "ChildTypeID", TableName = "sys_ItemTypeRelation")]
    public class ItemTypeRelation : BaseEntity<ItemTypeRelation>
    {
        public int ParentTypeID { get; set; }
        public int ChildTypeID { get; set; }
        public int Type { get; set; }
        public string Alias { get; set; }
        public int SortOrder { get; set; }
        public void Delete(int ID) 
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ParentID",ID);
            Db.ExecuteSpa("sp_DeleteChildItem", op);
        }
        public List<ItemTypeRelation> GetListChildItem(int typeID)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("ParentID", typeID);
            return Db.ExecuteSpa<ItemTypeRelation>("sp_GetChildTypeItem", op);
        }
    }
}