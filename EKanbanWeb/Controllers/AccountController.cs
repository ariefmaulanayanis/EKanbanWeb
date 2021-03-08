using EKanbanWeb.Data;
using EKanbanWeb.Models;
using EKanbanWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using EKanbanWeb.ViewModels;
using EKanbanWeb.Helpers;

namespace EKanbanWeb.Controllers
{
    public class AccountController : Controller
    {
        private EKanbanWebDBContext DbContext;
        private LoginInfo loginInfo;
        public AccountController(EKanbanWebDBContext context)
        {
            DbContext = context;
            loginInfo = new LoginInfo();
        }

        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            string encPassword = EncryptHelper.ConvertToEncrypt(model.Password);
            SyUser user = new SyUser();

            try
            {
                user = await DbContext.SyUser.FirstOrDefaultAsync(u => u.UserName == model.UserName && u.Password == encPassword);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                }
                else if (user.IsActive == false)
                {
                    ModelState.AddModelError(string.Empty, "User " + user.UserName + " is not active");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }

            if (ModelState.IsValid)
            {
                //store user info in session
                SyRole role = DbContext.SyRole.FirstOrDefault(a => a.RoleId == user.RoleId);
                var loginInfo = new LoginInfo
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    RealName = user.RealName,
                    RoleId = user.RoleId,
                    RoleName = role == null ? "" : role.RoleName
                };

                var serialisedLoginInfo = JsonConvert.SerializeObject(loginInfo);
                HttpContext.Session.SetString("LoginInfo", serialisedLoginInfo);

                //redirect to home
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        private void GetLoginInfo()
        {
            var sessionInfo = HttpContext.Session.GetString("LoginInfo");
            loginInfo = JsonConvert.DeserializeObject<LoginInfo>(sessionInfo);
            ViewBag.UserId = loginInfo.UserId;
            ViewBag.UserRealName = loginInfo.RealName;
            ViewBag.UserRoleName = loginInfo.RoleName;
        }

        public IActionResult ChangePassword()
        {
            GetLoginInfo();
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            return View(model);
        }

        //[HttpPost]
        //public IActionResult ChangePassword(ChangePasswordViewModel model)
        //{
        //    GetLoginInfo();
        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> ChangePasswordConfirm(ChangePasswordViewModel model)
        {
            //get user data
            var sessionInfo = HttpContext.Session.GetString("LoginInfo");
            LoginInfo loginInfo = JsonConvert.DeserializeObject<LoginInfo>(sessionInfo);

            SyUser user = DbContext.SyUser.Where(a => a.UserId == loginInfo.UserId).FirstOrDefault();
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User " + loginInfo.UserName + " is not recognized.");
            }
            else if (user.IsActive == false)
            {
                ModelState.AddModelError(string.Empty, "User " + loginInfo.UserName + " is not active.");
            }
            else
            {
                string decPassword = EncryptHelper.ConvertToDecrypt(user.Password);
                if (model.OldPassword != decPassword)
                {
                    ModelState.AddModelError("OldPassword", "Old Password is wrong.");
                }
                if (model.NewPassword == model.OldPassword)
                {
                    ModelState.AddModelError("NewPassword", "New Password is same with Old Password.");
                }
                if (model.ConfirmPassword != model.NewPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Confirm Password is different with New Password.");
                }
            }
            if (ModelState.IsValid)
            {
                user.Password = EncryptHelper.ConvertToEncrypt(model.NewPassword);
                user.EditDate = DateTime.Now;
                user.EditBy = loginInfo.UserId;
                DbContext.SyUser.Update(user);
                await DbContext.SaveChangesAsync();

                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            GetLoginInfo();
            return View("ChangePassword", model);
        }

        public IActionResult Logout()
        {
            GetLoginInfo();
            return View();
        }

        [HttpPost]
        public IActionResult LogoutConfirm()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult SessionExpired()
        {
            return View();
        }
    }
}
