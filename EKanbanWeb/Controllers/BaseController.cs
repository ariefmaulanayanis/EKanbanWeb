using EKanbanWeb.Data;
using EKanbanWeb.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Controllers
{
    public class BaseController : Controller
    {
        protected EKanbanWebDBContext DbContext { get; set; }
        protected LoginInfo LoginInfo { get; set; }

        //public IActionResult Index()
        //{
        //    return View();
        //}

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
                ViewBag.UserRoleName = LoginInfo.RoleName;
            }

            base.OnActionExecuting(context);
        }
    }
}
