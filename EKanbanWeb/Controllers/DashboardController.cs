using EKanbanWeb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class DashboardController : BaseController
    {
        public DashboardController(EKanbanWebDBContext context,
            ILogger<DashboardController> logger, IHttpContextAccessor accessor, IConfiguration configuration, IHostEnvironment hostEnvironment) :
            base(logger, accessor, configuration, hostEnvironment)
        {
            DbContext = context;
        }

        public IActionResult Index()
        {
            try
            {
                //Get Daily
                string sp = "sp_DashboardDaily";
                DataTable dt = SqlHelp.ExecQuery(sp, new List<Microsoft.Data.SqlClient.SqlParameter>());
                //
                ViewBag.Trolley = dt.Rows[0]["Trolley"].ToString();
                ViewBag.Kanban = dt.Rows[0]["Kanban"].ToString();
                ViewBag.TrolleyDelay = dt.Rows[0]["TrolleyDelay"].ToString();
                //For Chart Daily
                ViewBag.ChartDailyText = $"Trolley,Kanban,Delay";
                ViewBag.ChartDailyVal = $"{Convert.ToInt32(dt.Rows[0]["Trolley"].ToString())},{Convert.ToInt32(dt.Rows[0]["Kanban"].ToString())},{Convert.ToInt32(dt.Rows[0]["TrolleyDelay"].ToString())}";

                //Chart Monthly
                sp = "sp_DashboardMonthly";
                dt = SqlHelp.ExecQuery(sp, new List<Microsoft.Data.SqlClient.SqlParameter>());
                //For Chart Monthly
                ViewBag.ChartMonthlyText = string.Join(',', dt.AsEnumerable().Select(x => x.Field<DateTime>("Tgl").ToString("dd")).ToArray());
                ViewBag.ChartMonthlyTrolley = string.Join(',', dt.AsEnumerable().Select(x => x.Field<int>("Trolley")).ToArray());
                ViewBag.ChartMonthlyKanban = string.Join(',', dt.AsEnumerable().Select(x => x.Field<int>("Kanban")).ToArray());
                ViewBag.ChartMonthlyDelay = string.Join(',', dt.AsEnumerable().Select(x => x.Field<int>("TrolleyDelay")).ToArray());
            }
            catch(Exception ex)
            {
                LogHelp.WriteErrorLog(ex);
            }
            //Return
            IndexPrep();
            return View();
        }
    }
}
