using BlueMoon.DynWeb.Common;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System;
using System.IO;

namespace BlueMoon.DynWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        static object s_locker = new object();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BlueMoon.MVC.Controller.DataItemConverter = new DataItemConverter();
            BlueMoon.MVC.Controller.DownloadUrl = VirtualPathUtility.ToAbsolute("~/download/{0}?name={1}");
            BlueMoon.MVC.Controller.UploadFolder = Server.MapPath("~/App_Data/Upload/");
            ModelValidatorProviders.Providers.Clear();
            DofGenerator.DeleteFieldDefs();
            DofGenerator.DeleteViewDefs();
            //ModelValidatorProviders.Providers.Add(new DataAnnotationsModelValidatorProvider());
            //DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
        }
        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("Error at " + DateTime.UtcNow);
            sb.AppendLine("URL: " + Request.Url.AbsoluteUri);
            sb.AppendLine("IP: " + Request.UserHostAddress);
            sb.AppendLine("User-Agent: " + Request.UserAgent);
            int cnt = 0;
            do
            {
                sb.AppendLine(new string('\t', cnt) + "Message: " + ex.Message);
                sb.AppendLine(new string('\t', cnt) + "Stack trace \r\n" + ex.StackTrace);
                ex = ex.InnerException;
                cnt++;
            }
            while (ex != null);
            sb.AppendLine();
            lock (s_locker)
            {
                File.AppendAllText(Server.MapPath("~/App_Data/error.log"), sb.ToString());
            }
            
            //log the error!

        }
    }
}
