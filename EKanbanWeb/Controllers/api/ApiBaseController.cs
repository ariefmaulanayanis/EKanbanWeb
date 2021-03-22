using EKanbanWeb.Models;
using EKanbanWeb.Models.api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EKanbanWeb.Controllers.api
{
    public class ApiBaseController : ControllerBase
    {
        protected List<KanbanItem> UserKanban
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.AuthorizationToken))
                {
                    return null;
                }

                if (!KanbanFactory.ReqItems.ContainsKey(this.AuthorizationToken))
                {
                    return null;
                }

                var result = KanbanFactory.ReqItems[this.AuthorizationToken];

                return result.Item2;
            }
        }

        protected bool CheckAuthorization()
        {
            KanbanFactory.ClearStaleData();

            try
            {
                var ctx = HttpContext;
                if (ctx != null)
                {
                    if (string.IsNullOrWhiteSpace(this.AuthorizationToken))
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                if (!KanbanFactory.ReqItems.ContainsKey(this.AuthorizationToken))
                {
                    return false;
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        protected string AuthorizationToken
        {
            get
            {
                string authorizationToken = string.Empty;

                var ctx = HttpContext;
                if (ctx != null)
                {
                    authorizationToken = ctx.Request.Headers["Authorization"].ToString();
                }

                return authorizationToken;
            }
        }
    }
}
