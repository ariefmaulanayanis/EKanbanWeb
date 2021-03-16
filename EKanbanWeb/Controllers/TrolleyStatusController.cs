using EKanbanWeb.Data;
using EKanbanWeb.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Controllers
{
    public class TrolleyStatusController : BaseController
    {
        private MailHelper mailHelper;
        public TrolleyStatusController(EKanbanWebDBContext context,
            ILogger<DailyReportController> logger, IHttpContextAccessor accessor, IConfiguration configuration, IHostEnvironment hostEnvironment) :
            base(logger, accessor, configuration, hostEnvironment)
        {
            DbContext = context;
            mailHelper = new MailHelper(logger, hostEnvironment);
        }

        public IActionResult Index()
        {
            IndexPrep();
            return View();
        }

        public IActionResult SendMail()
        {
            string subject="Test Send Mail";
            string to="pepenk.en.ciel@gmail.com";
            string body="Ini hanya email test";
            if(!mailHelper.SendMail(subject, to, body))
            {
                ModelState.AddModelError(string.Empty, mailHelper.errMessage);
            }
            IndexPrep();
            return View("Index");
        }
    }
}
