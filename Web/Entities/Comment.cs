using BlueMoon.Business;
using BlueMoon.DynWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    [EntityTable(true, "ID" , TableName = "sys_Comment")]
    public class Comment : BaseEntity<Comment>
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public int AccountID { get; set; }
        public int EmployeeID { get; set; }
        public int ItemID { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CommentExt> GetListComment(int ItemID, int pageSize, int pageIndex)
        {
            ObjectParameter op = new ObjectParameter();
            op.Add("PageIndex", pageIndex);
            op.Add("PageSize", pageSize);
            op.Add("ItemID", ItemID);
            return Db.ExecuteSpa<CommentExt>("sp_GetListComment", op);
        }
    }
    public class CommentExt
    {
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalRows { get; set; }
    }
}