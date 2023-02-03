using BlueMoon.Business;
using BlueMoon.MVC.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BlueMoon.DynWeb.Common;
namespace BlueMoon.DynWeb.Entities
{
    public class ApprovalState
    {
        public const string APPROVED = "approved";
        public const string SUBMITTED = "submitted";
        public const string REJECTED = "rejected";
    }
    [EntityTable(true, "ID", TableName = "sys_ApprovalRule")]
    public class ApprovalRule : BaseEntity<ApprovalRule>
    {
        public int ID { get; set; }
        [Input(Validation = "[{\"rule\": \"required\", \"msg\": \"Rule name is required\"}]")]
        public string Name { get; set; }
        [Input(Validation = "[{\"rule\": \"required\", \"msg\": \"Permissions is required\"}]")]
        public string Permissions { get; set; }
        [Input(Validation = "[{\"rule\": \"required\", \"msg\": \"Item type is required\"}]")]
        public int AppliedItemType { get; set; }
        [Input(Validation = "[{\"rule\": \"required\", \"msg\": \"Query state is required\"}]")]
        public string QueryState { get; set; }
        [Input(Validation = "[{\"rule\": \"required\", \"msg\": \"Next state is required\"}]")]
        public string NextState { get; set; }

        public string Queries { get; set; }//Json for queries

        public string Actions { get; set; }//Json for cols

        public int CreatedBy { get; set; }
    }
    public class Action
    {

        public string prop { get; set; }
        public string value { get; set; }
    }

    [EntityTable(false, "ItemId", "Actor", "State", "CreatedTime", TableName = "sys_ItemApproval")]
    public class ItemApproval : BaseEntity<ItemApproval>
    {
        public int ItemId { get; set; }
        public int Actor { get; set; }
        public string State { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Comment { get; set; }

        [IgnoreProperty]
        public string Approver { get; set; }
    }
}