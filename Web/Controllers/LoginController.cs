using BlueMoon.DynWeb.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using BlueMoon.Business;
using System.Web.Security;
using BlueMoon.DynWeb.Entities;

namespace BlueMoon.DynWeb.Controllers
{

    public class LoginController : Controller
    {
        public ActionResult Index(Account model, string returnUrl)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                return View(model);
            }
            else
            {
                if (ConfigurationManager.ConnectionStrings["ADConnectionString"] != null && Membership.ValidateUser(model.Username, model.Password))
                {

                    Account account = null;
                    if (model.GetAccount(model.Username))
                    {
                        account = model;
                    }
                    else
                    {
                        Role role = new Role();
                        role.GetByRoleCode("user");//default role

                        account = new Account();
                        account.Username = model.Username;
                        account.Password = "";
                        account.RoleID = role.ID;
                        account.Insert();
                    }
                    account.Permission = new Permission().GetPermissionOfRole(account.RoleID);
                    SetAuthLogin(account);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return Redirect(FormsAuthentication.DefaultUrl);
                }
                else
                {
                    //var account = model.CheckAccount(model.Username, Encryptor.MD5Hash(model.Password));
                    if (model.CheckAccount(model.Username, Encryptor.MD5Hash(model.Password)))
                    {
                        Account account = model;
                        account.Permission = new Permission().GetPermissionOfRole(account.RoleID);
                        SetAuthLogin(account);
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return Redirect("~/");
                    }
                    else
                    {
                        model.Password = "";
                        ModelState.AddModelError("", "Invalid Username/Password");
                    }
                }
                return View(model);
            }
        }
        void SetAuthLogin(Account account)
        {
            SessionManager.CurrentUser = account;
            FormsAuthentication.SetAuthCookie(account.ID.ToString(), false);
        }
        [Route("~/logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            SessionManager.CurrentUser = null;
            return Redirect("~/login");
        }
    }
}