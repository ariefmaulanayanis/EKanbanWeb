using EKanbanWeb.Data;
using EKanbanWeb.Helpers;
using EKanbanWeb.Models;
using EKanbanWeb.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Controllers
{
    public class BaseController : Controller
    {
        protected EKanbanWebDBContext DbContext { get; set; }
        protected LoginInfo LoginInfo { get; set; }
        protected FormHelper FrmHelp { get; set; }
        protected SqlHelper SqlHelp { get; set; }
        protected LogHelper LogHelp { get; set; }
        protected ILogger Logger { get; set; }
        protected IHttpContextAccessor Accessor { get; set; }
        protected IConfiguration Configuration { get; set; }
        public IHostEnvironment HostEnvironment { get; set; }

        public BaseController(ILogger logger, IHttpContextAccessor accessor, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Logger = logger;
            Accessor = accessor;
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
            SqlHelp = new SqlHelper(Configuration, HostEnvironment);
            LogHelp = new LogHelper(Logger);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionInfo = HttpContext.Session.GetString("LoginInfo");
            if (string.IsNullOrEmpty(sessionInfo))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary{
                        {"controller","Account" },{"action","Login"}
                    });
                return;
            }
            else
            {
                LoginInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginInfo>(sessionInfo);
                ViewBag.UserRealName = LoginInfo.RealName;
                ViewBag.UserRoleId = LoginInfo.RoleId;
                ViewBag.UserRoleName = LoginInfo.RoleName;
                ViewBag.UserLineId = LoginInfo.LineId;
                ViewBag.UserLineName = LoginInfo.LineName;
            }

            base.OnActionExecuting(context);
        }

        protected void IndexPrep()
        {
            GetMenu();
            GetPageAccess();
            GetButtonExport();
            GetFilterCardHeader();
        }

        protected void GetMenu()
        {
            string sp = "sp_GetUserMenu";
            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter("@roleId", LoginInfo.RoleId)
            };
            DataTable dt = SqlHelp.ExecQuery(sp, param);
            List<MenuViewModel> menuList = SqlHelp.ConvertToList<MenuViewModel>(dt);

            ViewBag.MenuList = menuList;
        }

        protected void GetPageAccess()
        {
            ViewBag.SearchButton = @"<button type=""submit"" class=""btn btn-primary"" id=""btnSearch"">
                <span class=""fas fa-search"" style=""padding-right:10px;""></span>Search Data
            </button>";
            ViewBag.ActionColumnVisible = "visible: false";
            ViewBag.ActionColumnHtml = "";

            //get authorization
            string linkUrl = @"\" + GetControllerName() + @"\Index";
            SyMenu menu = DbContext.SyMenu.Where(a => a.LinkUrl == linkUrl).FirstOrDefault();
            SyRoleAccess pageAccess = new SyRoleAccess();
            if (menu != null)
            {
                pageAccess = DbContext.SyRoleAccess.Where(a => a.RoleId == LoginInfo.RoleId && a.MenuId == menu.MenuId).FirstOrDefault();
                if (pageAccess != null)
                {
                    if (pageAccess.CanEdit == true || pageAccess.CanDelete == true)
                    {
                        ViewBag.ActionColumnVisible = "visible: true";
                        ViewBag.ActionColumnHtml = "<th class='text-center'>Action</th>";
                    }
                }
            }
            ViewBag.PageAccess = pageAccess;
        }

        private string GetControllerName()
        {
            return GetType().Name[0..^10];
        }

        private void GetButtonExport()
        {
            ViewBag.ButtonExportJS = @"$('#btnExcel').click(function () {
                $('.buttons-excel').click();
            });

            $('#btnPdf').click(function () {
                $('.buttons-pdf').click();
            });

            $('#btnPrint').click(function () {
                $('.buttons-print').click();
            });";

            ViewBag.ButtonExportHide = @"initComplete: function () {
                $('.buttons-excel').hide();
                $('.buttons-pdf').hide();
                $('.buttons-print').hide();
            },";

            ViewBag.ButtonExportHtml = @"<button type=""button"" class=""btn btn-primary"" id=""btnExcel"">
                <span class=""far fa-file-excel"" style=""padding-right:10px;""></span>Export to Excel
            </button>
            <button type=""button"" class=""btn btn-primary"" id=""btnPdf"">
                <span class=""far fa-file-pdf"" style=""padding-right:10px;""></span>Export to PDF
            </button>
            <button type=""button"" class=""btn btn-primary"" id=""btnPrint"">
                <span class=""fas fa-print"" style=""padding-right:10px;""></span>Print
            </button>";
        }

        private string GetCardHeader(string headTitle)
        {
            return @"<div class=""card-header"">
                <h6 class=""card-title"">" + headTitle + @"</h6>
                <div class=""card-tools"">
                    <button type = ""button"" class=""btn btn-tool"" data-card-widget=""collapse"">
                        <i class=""fas fa-minus""></i>
                    </button>
                </div>
            </div>";
        }
        private void GetFilterCardHeader()
        {
            ViewBag.CardHeadFilter = GetCardHeader("Filter Data");
        }
    }
}
