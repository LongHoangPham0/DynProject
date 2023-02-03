using BlueMoon.DynWeb.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BlueMoon.MVC.Controls;
namespace BlueMoon.DynWeb.Controllers
{
    public abstract class Controller : MVC.Controller
    {
        public static ActionResult UnauthorizedResult(string contentType)
        {
            if (contentType == "application/json") return new JsonResult()
            {
                Data = new { msg = "Unauthorized" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            else if (System.Web.HttpContext.Current.Request.IsAuthenticated && SessionManager.CurrentUser != null) return new RedirectToRouteResult(
                new RouteValueDictionary(
                    new
                    {
                        controller = "error",
                        action = "unauthorized"
                    })
                );
            else return new RedirectToRouteResult(
                new RouteValueDictionary(
                    new
                    {
                        controller = "login",
                        action = "index"
                    })
                );
        }
        protected ActionResult Unauthorized(string contentType = null)
        {
            Response.StatusCode = 403;
            return UnauthorizedResult(contentType ?? Request.ContentType);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (Request.HttpMethod == "GET" && (Request.Cookies["AlreadyLoaded"] == null || ((filterContext.ActionDescriptor.HasCustomAttribute<ShowLoadingAttribute>(true) || filterContext.ActionDescriptor.ControllerDescriptor.HasCustomAttribute<ShowLoadingAttribute>(true)) && TempData["ReloadUrl"] == null)))
            {
                TempData["ReloadUrl"] = Request.RawUrl;
                Response.Cookies.Add(new HttpCookie("AlreadyLoaded", "1") { HttpOnly = true });
                filterContext.Result = new RedirectResult("~/loading");

            }
        }
    }
    public class ShowLoadingAttribute : Attribute { }
}