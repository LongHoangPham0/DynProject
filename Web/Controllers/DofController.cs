using BlueMoon.Business;
using BlueMoon.MVC.Controls;
using BlueMoon.DynWeb.Common;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Linq;
using KeyValue = System.Collections.Generic.Dictionary<string, object>;
using System.Text.RegularExpressions;
using BlueMoon.DynWeb.Entities;

namespace BlueMoon.DynWeb.Controllers
{
    public class DofController : Controller
    {
        
        [Route("~/dof/fields")]
        public ActionResult GetFields()
        {
            return Content(DofGenerator.GetFields(), "application/javascript");
        }
        static readonly Regex reg_PPP = new Regex(@"\{"".*?"":""\[v\]""\}", RegexOptions.Compiled);
        
        [Route("~/dof/views")]
        public ActionResult GetViews()
        {
            return Content(DofGenerator.GetViews(), "application/javascript");
        }
    }

}