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
    public class TrolleyStatusController : BaseController
    {
        //private MailHelper mailHelper;
        public TrolleyStatusController(EKanbanWebDBContext context,
            ILogger<DailyReportController> logger, IHttpContextAccessor accessor, IConfiguration configuration, IHostEnvironment hostEnvironment) :
            base(logger, accessor, configuration, hostEnvironment)
        {
            DbContext = context;
            FrmHelp = new FormHelper(DbContext);
            //mailHelper = new MailHelper(logger, hostEnvironment);
        }

        public IActionResult Index()
        {
            IndexPrep();
            ViewBag.Lines = FrmHelp.GetLineList();
            return View();
        }

        public string GetData(int lineId)
        {
            try
            {
                string sp = "sp_KanbanStatusGetData";
                List<SqlParameter> param = new List<SqlParameter>
                {
                    new SqlParameter("@lineId", lineId)
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

        //public IActionResult SendMail()
        //{
        //    string subject="Test Send Mail";
        //    string to="pepenk.en.ciel@gmail.com";
        //    string body="Ini hanya email test";
        //    if(!mailHelper.SendMail(subject, to, body))
        //    {
        //        ModelState.AddModelError(string.Empty, mailHelper.errMessage);
        //    }
        //    IndexPrep();
        //    return View("Index");
        //}
    }
}
