using EKanbanWeb.Data;
using EKanbanWeb.Helpers;
using EKanbanWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Controllers
{
    public class UserController : BaseController
    {
        public UserController(EKanbanWebDBContext context,
            ILogger<UserController> logger, IHttpContextAccessor accessor, IConfiguration configuration, IHostEnvironment hostEnvironment) :
            base(logger, accessor, configuration, hostEnvironment)
        {
            DbContext = context;
            FrmHelp = new FormHelper(DbContext);
        }

        public IActionResult Index()
        {
            IndexPrep();
            return View();
        }

        [HttpPost]
        public string GetData(byte roleId)
        {
            try
            {
                string sp = "sp_UserGetData";
                List<SqlParameter> param = new List<SqlParameter>();
                DataTable dt = SqlHelp.ExecQuery(sp, param);

                string json = JsonConvert.SerializeObject(dt);
                return json;
            }
            catch (Exception e)
            {
                LogHelp.WriteErrorLog(e);
            }
            return "";
        }

        [HttpGet]
        public IActionResult Detail(Int16 id)
        {
            try
            {
                SyUser data;
                if (id == 0)
                    data = new SyUser()
                    {
                        IsActive = true //default Active
                    };
                else
                {
                    data = DbContext.SyUser.Where(a => a.UserId == id).FirstOrDefault();
                    if (data == null) return NotFound();
                    else data.Password = EncryptHelper.ConvertToDecrypt(data.Password);
                }
                SetViewBag();
                return View(data);
            }
            catch (Exception e)
            {
                LogHelp.WriteErrorLog(e);
            }
            return View();
        }

        private void SetViewBag()
        {
            ViewBag.Roles = FrmHelp.GetRoleList();
            ViewBag.Lines = FrmHelp.GetLineList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SyUser model)
        {
            bool status = false;

            //validation
            bool isExist;
            isExist = DbContext.SyUser.Any(a => a.UserId != model.UserId && a.UserName == model.UserName);
            if (isExist) ModelState.AddModelError("UserName", "User " + model.UserName + " is already existed.");

            if (ModelState.IsValid)
            {
                try
                {
                    SyUser data;
                    if (model.UserId == 0)
                        data = new SyUser();
                    else
                    {
                        data = await DbContext.SyUser.FirstOrDefaultAsync(a => a.UserId == model.UserId);
                        if (data == null)
                            return NotFound();
                    }

                    data.UserName = model.UserName;
                    data.Password = EncryptHelper.ConvertToEncrypt(model.Password);
                    data.RealName = model.RealName;
                    data.RoleId = model.RoleId;
                    data.LineId = model.LineId;
                    data.IsActive = model.IsActive;

                    if (model.UserId == 0)
                    {
                        data.InsertDate = DateTime.Now;
                        data.InsertBy = LoginInfo.UserId;
                        DbContext.SyUser.Add(data);
                    }
                    else
                    {
                        data.EditDate = DateTime.Now;
                        data.EditBy = LoginInfo.UserId;
                        DbContext.SyUser.Update(data);
                    }
                    DbContext.SaveChanges();
                    status = true;
                }
                catch (Exception e)
                {
                    LogHelp.WriteErrorLog(e);
                }
                return Json(new { status });
            }
            else
            {
                SetViewBag();
                return Json(new { status, html = ViewHelper.RenderRazorViewToString(this, "Detail", model) });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(short id)
        {
            var data = await DbContext.SyUser.FirstOrDefaultAsync(a => a.UserId == id);
            if (data != null)
            {
                ViewBag.RoleName = FrmHelp.GetRoleName(data.RoleId);
                ViewBag.LineName = FrmHelp.GetLineName(data.LineId);
                return View(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            bool status = false;
            var v = await DbContext.SyUser.FirstOrDefaultAsync(a => a.UserId == id);
            if (v != null)
            {
                DbContext.SyUser.Remove(v);
                DbContext.SaveChanges();
                status = true;
            }
            else
            {
                return NotFound();
            }
            return Json(new { status });
        }
    }
}
