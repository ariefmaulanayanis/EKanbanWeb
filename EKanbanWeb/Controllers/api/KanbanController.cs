using EKanbanWeb.Data;
using EKanbanWeb.Models;
using EKanbanWeb.Models.api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EKanbanWeb.Controllers.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanbanController : ApiBaseController
    {
        private EKanbanWebDBContext DbContext { get; set; }

        public KanbanController(EKanbanWebDBContext context)
        {
            DbContext = context;
        }

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

        [HttpPut("{kanbanReqId}")]
        public HttpResponseMessage Put(int kanbanReqId, [FromBody] KanbanHeader header)
        {
            try
            {
                var authorized = CheckAuthorization();
                if (!authorized)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                if (!ModelState.IsValid)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                //if (string.IsNullOrEmpty(header.KanbanReqId))
                //{
                //    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                //}

                //Console.WriteLine($"PUT /api/kanban/{kanbanReqId}");
                //Console.WriteLine(JsonSerializer.Serialize(header));


                //var userBooks = UserBooks;
                //var existingBook = userBooks.SingleOrDefault(x => x.ISBN == isbn);
                KanbanRequest kanban = DbContext.KanbanRequest.Where(a => a.KanbanReqId == kanbanReqId).FirstOrDefault();
                if (kanban != null)
                {
                    kanban.StatusId = 4;
                    kanban.ReturnDateTime = DateTime.Now;
                    kanban.EditBy = 0;
                    kanban.EditDate = DateTime.Now;
                    DbContext.KanbanRequest.Update(kanban);
                    DbContext.SaveChanges();
                }

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
