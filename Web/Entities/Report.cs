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
    [EntityTable(true, "ID", TableName = "sys_Report")]
    public class Report : BaseEntity<Report>
    {
        public int ID { get; set; }
        [Input(Validation = "[{\"rule\": \"required\", \"msg\": \"Report name is required\"}]")]
        public string Name { get; set; }
        [Input("Display property", Validation = "[{\"rule\": \"required\", \"msg\": \"Data source is required\"}]")]
        public int DataSource { get; set; }

        public string Queries { get; set; }//Json for queries

        public string DisplayColumns { get; set; }//Json for cols
        public string Sorts { get; set; }//Json for cols
        

        public List<DataItem> GetData(int type, string[] cols, Query[] queries, string orderBy, int page, int pageSize)
        {
            ObjectParameter objectParameter = new ObjectParameter();
            objectParameter.Add("PageIndex", page);
            objectParameter.Add("PageSize", pageSize);
            objectParameter.Add("Type", type);
            StringBuilder bd = new StringBuilder("1=1");
            foreach(var q in queries)
            {
                bd.Append(" AND " + q);
            }
            if(!string.IsNullOrEmpty(orderBy)) objectParameter.Add("OrderBy", orderBy); 
            objectParameter.Add("QueryFilter", bd.ToString());
            if (cols != null && cols.Length>0) objectParameter.Add("Cols", string.Join(",", cols));
            return Db.ExecuteSpa("[sp_ViewReport]", objectParameter);
        }
    }
    public class Query
    {

        public string propId { get; set; }
        public string @operator { get; set; }
        public string value { get; set; }

        public override string ToString()
        {
            string val = value ?? "";
            switch (@operator)
            {
                case "like":
                    val = "%" + val.AntiSQLInjection() + "%";
                    break;
            }
            return string.Format("{0} {1} N'{2}'", propId, @operator, val.AntiSQLInjection());
        }
    }
    public class OrderBy
    {

        public string propId { get; set; }
        public string direction { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", propId, direction);
        }
    }
}