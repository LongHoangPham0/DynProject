using BlueMoon.DynWeb.Common;
using BlueMoon.DynWeb.Entities;
using BlueMoon.MVC.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueMoon.DynWeb.Controllers
{
    public class HomeController : Controller
    {
        [Route("~/loading")]
        public ActionResult Loading()
        {
            string reloadUrl = (string)TempData["ReloadUrl"];
            ViewBag.RedirectScript = reloadUrl == null ? "" : "location.href='" + reloadUrl + "';";
            return View();
        }
        [Route("~/welcome")]
        public ActionResult Welcome()
        {
            return View();
        }
        public ActionResult Index()
        {
            return RedirectToAction("welcome");
        }
        [Route("~/{type}/list")]
        public ActionResult Index(string type)
        {
            ItemType itemType = CacheManager.AllItemTypes[type];
            if (!SessionManager.CheckItemPermission(itemType)) return Unauthorized();
            ViewBag.CurrentType = type;
            ViewBag.Editable = SessionManager.CurrentUser == null ? false : (string.IsNullOrEmpty(itemType.ModPermission) ? true : SessionManager.CheckItemPermission(itemType, true));
            //check if approval list is available
            ViewBag.Approval = false;
            ViewBag.IsApprover = false;
            if (itemType.ApprovalProcess)
            {
                ViewBag.Approval = true;
                ApprovalRule rule = new ApprovalRule();
                var rules = rule.GetAll(null, "[AppliedItemType] = " + itemType.ID);
                if (rules != null)
                    foreach (var r in rules)
                    {
                        if (SessionManager.CurrentUser.HasPermission(r.Permissions.Split(new char[] { ',', ';' })))
                        {
                            ViewBag.IsApprover = true;
                            break;
                        }
                    }


            }

            return View(itemType);
        }
        [AuthorizeUser]
        [Route("~/{type}/detail")]
        public ActionResult Detail(string type)
        {
            ItemType itemType = CacheManager.AllItemTypes[type];
            int itemId = SessionManager.CurrentUser.GetLinkedItemId(itemType.ID);
            if (itemId == 0) return Unauthorized();
            ViewBag.CurrentType = type;
            ViewBag.Editable = SessionManager.CheckItemPermission(itemType, true);
            ViewBag.ItemId = itemId.ToString().Encrypt();
            ViewBag.Approval = false;
            ViewBag.IsApprover = false;
            return View(itemType);
        }

        [AuthorizeUser(Permissions = new string[] {Permission.ACCESS_REPORTING, Permission.MANAGE_REPORTING})]
        [Route("~/reports")]
        public ActionResult Reports()
        {
            ViewBag.Editable = SessionManager.CurrentUser.HasPermission(Permission.MANAGE_REPORTING);
            return View();
        }
        [AuthorizeUser(Permission = Permission.MANAGE_APPROVAL)]
        [Route("~/approval/rules")]
        public ActionResult ApprovalRules()
        {
            return View();
        }
    }
}