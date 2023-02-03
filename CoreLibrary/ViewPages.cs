using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BlueMoon.MVC.Controls
{
    public abstract class WebViewPage<X> : System.Web.Mvc.WebViewPage<X>
    {
        public override void InitHelpers()
        {
            base.InitHelpers();
        }
        public MvcHtmlString RenderScripts()
        {
            return Html.RenderRegisteredScripts();
        }
        public MvcHtmlString RenderStyles()
        {
            return Html.RenderRegisteredStyles();
        }
    }
}
