using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueMoon.DynWeb.Models
{
    public class AccountModel
    {
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalRows { get; set; }
    }
}