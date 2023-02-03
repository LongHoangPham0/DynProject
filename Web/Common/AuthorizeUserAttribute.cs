using BlueMoon.DynWeb.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using BlueMoon.MVC.Controls;
namespace BlueMoon.DynWeb.Common
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public string Permission { get; set; }
        public string[] Permissions {
            get
            {
                if (Permission == null) return null;
                else return Permission.Split(new char[] { ',', ';' });
            }
            set
            {
                Permission = string.Join(",", value);
            }
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            Permission = null;
            var atts = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AuthorizeUserAttribute>(true);
            if (atts != null) foreach (var oat in atts)
            {
                if (oat.Permissions != null)
                {
                    Permissions = oat.Permissions;
                }
                else
                {
                    Permission = oat.Permission;
                }

            }
            atts = filterContext.ActionDescriptor.GetCustomAttributes<AuthorizeUserAttribute>(true);
            if (atts != null)
            {
                Permission = null;
                foreach (var oat in atts)
                {
                    if (oat.Permissions != null)
                    {
                        Permissions = oat.Permissions;
                    }
                    else
                    {
                        Permission = oat.Permission;
                    }

                }
            }
                
            base.OnAuthorization(filterContext);
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (Permission == "[ItemAccess]" || Permission == "[ModAccess]")
            {
                var stream = httpContext.Request.InputStream;
                var reader = new StreamReader(stream);
                string jsonPostData = reader.ReadToEnd();
                stream.Position = 0;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Dictionary<string, object> req = serializer.DeserializeObject(jsonPostData) as Dictionary<string, object>;
                if (req != null && req.ContainsKey("type"))
                {
                    ItemType itemType = CacheManager.AllItemTypes[(string)req["type"]];
                    return SessionManager.CheckItemPermission(itemType, Permission == "[ModAccess]");
                }
                else return true;
            }
            else if (string.IsNullOrEmpty(Permission)) return httpContext.Request.IsAuthenticated && SessionManager.CurrentUser != null;
            else
            {
                if (SessionManager.CurrentUser != null)
                {
                    return SessionManager.CurrentUser.HasPermission(Permissions);
                }
            }

            return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Response.StatusCode = 403;
            var request = filterContext.RequestContext.HttpContext.Request;
            string contentType = request.ContentType;
            if (request.AcceptTypes.Contains("application/json")) contentType = "application/json";
            filterContext.Result = Controllers.Controller.UnauthorizedResult(contentType);
        }
    }
}