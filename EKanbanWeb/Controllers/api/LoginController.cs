using EKanbanWeb.Data;
using EKanbanWeb.Models.api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Controllers.api
{
    [ApiController]
    [Route("api/kanban/[controller]")]
    public class LoginController : Controller
    {
        private EKanbanWebDBContext DbContext { get; set; }
        public LoginController(EKanbanWebDBContext context)
        {
            DbContext = context;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var authorizationToken = Guid.NewGuid().ToString();

                KanbanFactory factory = new KanbanFactory(DbContext);
                factory.Initialize(authorizationToken);

                return new JsonResult(authorizationToken);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
