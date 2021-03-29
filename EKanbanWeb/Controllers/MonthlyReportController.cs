using EKanbanWeb.Data;
using EKanbanWeb.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    public class MonthlyReportController : BaseController
    {
        public MonthlyReportController(EKanbanWebDBContext context,
            ILogger<MonthlyReportController> logger, IHttpContextAccessor accessor, IConfiguration configuration, IHostEnvironment hostEnvironment) :
            base(logger, accessor, configuration, hostEnvironment)
        {
            DbContext = context;
            FrmHelp = new FormHelper(DbContext);
        }

        public IActionResult Index()
        {
            IndexPrep();
            ViewBag.Month = DateTime.Today.ToString("yyyy-MM");
            ViewBag.Lines = FrmHelp.GetLineList();
            return View();
        }

        public string GetData(int lineId, string month)
        {
            try
            {
                string sp = "sp_MonthlyReportGetData";
                List<SqlParameter> param = new List<SqlParameter>
                {
                    new SqlParameter("@lineId", lineId),
                    new SqlParameter("@month", month??"")
                };
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
    }
}
