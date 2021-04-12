using EKanbanWeb.Data;
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
            IndexPrep();
            return View();
        }
    }
}
