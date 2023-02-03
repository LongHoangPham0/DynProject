using BlueMoon.MVC.Controls;
using BlueMoon.Business;
using BlueMoon.DynWeb.Common;
using BlueMoon.DynWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlueMoon.DynWeb.Entities;

namespace BlueMoon.DynWeb.Controllers
{
    [AuthorizeUser(Permission = Permission.MANAGE_SYSTEM)]
    public class SystemController : Controller
    {
        [AuthorizeUser]
        [Route("~/system/profile")]
        public ActionResult UserProfile(SystemModel model)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                model.Account = SessionManager.CurrentUser;
            }
            else
            {
                Account account = new Account();
                account.ID = model.Account.ID;
                account.Get();
                if (model.Account.ResetPwd)
                {
                    model.Account.Password = Encryptor.MD5Hash(model.Account.Password);
                }
                else
                {
                    model.Account.Password = account.Password;   
                }

                model.Account.Username = account.Username;
                model.Account.RoleID = account.RoleID;
                model.Account.LinkedIDs = account.LinkedIDs;
                model.Account.Update();
            }
            return View(model);
        }
        [Route("~/system/role/list")]
        public ActionResult ListRole(SystemModel model)
        {
            Role role = new Role();
            model.ListRole = role.GetListRole();
            return View(model);
        }
        [Route("~/system/role/manage")]
        public ActionResult ManageRole(SystemModel model)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                if (model.Role == null) model.Role = new Role();
                Permission permission = new Permission();
                model.ListPermission = permission.GetListPermission();
                model.PermissionSelected = new List<int>();
                if (model.Role.ID > 0)
                {
                    model.Role.Get();
                    List<Permission> lstPermissionOfRole = new List<Permission>();
                    lstPermissionOfRole = permission.GetPermissionOfRole(model.Role.ID);
                    if (lstPermissionOfRole != null)
                    {
                        for (int i = 0; i < lstPermissionOfRole.Count; i++)
                        {
                            model.PermissionSelected.Add(lstPermissionOfRole[i].ID);
                        }
                    }
                }
                model.ReturnUrl = Url.Action("listrole", "system");
            }
            else
            {
                if (model.Role.ID == 0)
                {
                    model.Role.Insert();
                    if (model.PermissionSelected != null)
                    {
                        RolePermission rolePermission = new RolePermission();
                        for (int i = 0; i < model.PermissionSelected.Count; i++)
                        {
                            rolePermission.RoleID = model.Role.ID;
                            rolePermission.PermissionID = model.PermissionSelected[i];
                            rolePermission.Insert();
                        }
                    }
                }
                else
                {
                    Role role = new Role();
                    role.ID = model.Role.ID;
                    role.Get();
                    role = model.Role;
                    role.Update();

                    role.DeleteRolePermission(role.ID);
                    if (model.PermissionSelected != null)
                    {
                        RolePermission rolePermission = new RolePermission();
                        for (int i = 0; i < model.PermissionSelected.Count; i++)
                        {
                            rolePermission.RoleID = model.Role.ID;
                            rolePermission.PermissionID = model.PermissionSelected[i];
                            rolePermission.Insert();
                        }
                    }
                }
                return Redirect(model.ReturnUrl);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteRole(int ID)
        {
            Role role = new Role();
            role.Delete(ID);
            return RedirectToAction("ListRole");
        }
        [Route("~/system/permission/list")]
        public ActionResult ListPermission(SystemModel model)
        {
            Permission permission = new Permission();
            model.ListPermission = permission.GetListPermission();
            return View(model);
        }
        [Route("~/system/permission/manage")]
        public ActionResult ManagePermission(SystemModel model)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                if (model.Permission == null) model.Permission = new Permission();
                if (model.Permission.ID > 0)
                {
                    model.Permission.Get();
                }

                model.ReturnUrl = Url.Action("listpermission", "system");
            }
            else
            {
                if (model.Permission.ID == 0)
                {
                    model.Permission.Insert();
                }
                else
                {
                    Permission permission = new Permission();
                    permission.ID = model.Permission.ID;
                    permission.Get();
                    permission.Name = model.Permission.Name;
                    permission.Description = model.Permission.Description;
                    permission.Update();

                }
                return Redirect(model.ReturnUrl);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult DeletePermission(int ID)
        {
            Permission permission = new Permission();
            permission.Delete(ID);
            return RedirectToAction("ListPermission");
        }
        [Route("~/system/account/list")]
        public ActionResult ListAccount(SystemModel model)
        {
            Account lstAccount = new Account();
            model.ListAccount = lstAccount.GetListAccount();
            return View(model);
        }
        [Route("~/system/account/manage")]
        public ActionResult ManageAccount(SystemModel model)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                if (model.Account == null) model.Account = new Account();
                if (model.Account.ID > 0)
                {
                    model.Account.Get();
                    //update LinkedIds with text;
                    model.Account.LinkedIDs = model.Account.GetLinkedIdsForUIControl();

                }
                Role role = new Role();
                Permission permission = new Permission();
                model.ListRole = role.GetListRole();
                model.ListPermission = permission.GetListPermission();
                model.ReturnUrl = Url.Action("listaccount", "system");
            }
            else
            {
                model.Account.LinkedIDs = model.Account.LinkedIDs.DecryptAll();
                model.Account.LinkedIDs = model.Account.RemoveUIInfoFromLinkedIds();
                if (model.Account.ID == 0)
                {
                    model.Account.Password = Encryptor.MD5Hash(model.Account.Password);
                    model.Account.Insert();
                }
                else
                {
                    

                    if (model.Account.ResetPwd)
                    {
                        model.Account.Password = Encryptor.MD5Hash(model.Account.Password);
                    }
                    else
                    {
                        Account account = new Account();
                        account.ID = model.Account.ID;
                        account.Get();
                        model.Account.Password = account.Password;
                    }
                    
                    model.Account.Update();
                }
                return Redirect(model.ReturnUrl);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteAccount(int ID)
        {
            Account account = new Account();
            account.ID = ID;
            account.Get();
            account.Delete();
            return RedirectToAction("ListAccount");
        }
    }
}