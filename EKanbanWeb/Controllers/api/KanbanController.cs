using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Controllers.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanbanController : ApiBaseController
    {
        [HttpGet]
        public ActionResult Get()
        {
            var authorized = CheckAuthorization();
            if (!authorized)
            {
                return Unauthorized();
            }
            Console.WriteLine("GET /api/kanban");
            return new JsonResult(UserKanban);
        }
    }
}
