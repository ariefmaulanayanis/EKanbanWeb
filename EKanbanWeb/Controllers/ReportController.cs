using AspNetCore.Reporting;
using EKanbanWeb.Data;
using EKanbanWeb.Helpers;
using EKanbanWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private EKanbanWebDBContext DbContext;
        private SqlHelper SqlHelp { get; set; }

        public ReportController(IWebHostEnvironment webHostEnvironment, EKanbanWebDBContext context, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            
            DbContext = context;
            SqlHelp = new SqlHelper(configuration, hostEnvironment);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Print()
        {
            var dt = new DataTable();


            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\Report1.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("rp1", "Welcome");

            LocalReport localReport = new LocalReport(path);

            string sp = "select * from SyMenu";
            List<SqlParameter> param = new List<SqlParameter>();
            dt = SqlHelp.ExecQuery(sp, param);
            localReport.AddDataSource("dsMenu", dt);

            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimtype);
            return File(result.MainStream, "application/pdf");
        }

        public IActionResult Print2()
        {
            var dt = new DataTable();


            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\eKanbanReport.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("rp1", "Welcome");

            LocalReport localReport = new LocalReport(path);

            string sql = "select * from v_KanbanRequest";
            dt = SqlHelp.ExecQueryCommand(sql);
            localReport.AddDataSource("v_KanbanRequest", dt);
            string sql2 = "select * from v_KanbanReqItem";
            dt = SqlHelp.ExecQueryCommand(sql2);
            localReport.AddDataSource("v_KanbanReqItem", dt);

            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimtype);
            return File(result.MainStream, "application/pdf");
        }

        private IActionResult File()
        {
            throw new NotImplementedException();
        }
    }
}
