using EKanbanWeb.Data;
using EKanbanWeb.Helpers;
using EKanbanWeb.Models;
using EKanbanWeb.Models.View;
using EKanbanWeb.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
    public class KanbanRequestController : BaseController
    {
        public KanbanRequestController(EKanbanWebDBContext context,
            ILogger<KanbanRequestController> logger, IHttpContextAccessor accessor, IConfiguration configuration, IHostEnvironment hostEnvironment) :
            base(logger, accessor, configuration, hostEnvironment)
        {
            DbContext = context;
            FrmHelp = new FormHelper(DbContext);
        }

        public IActionResult Index()
        {
            IndexPrep();
            switch (LoginInfo.RoleId)
            {
                case 1: 
                    ViewBag.Lines = FrmHelp.GetLineList();
                    ViewBag.Statuses = FrmHelp.GetKanbanStatusList(1);
                    break;
                case 2:
                    ViewBag.Lines = FrmHelp.GetLineList((int)LoginInfo.LineId);
                    ViewBag.Statuses = FrmHelp.GetKanbanStatusList(1);
                    break;
                case 3:
                    ViewBag.Lines = FrmHelp.GetLineList();
                    ViewBag.Statuses = FrmHelp.GetKanbanStatusList(3);
                    break;
            }
            return View();
        }

        public string GetData(string partNo,int lineId,byte statusId)
        {
            try
            {
                string sp = "sp_KanbanGetData";
                List<SqlParameter> param = new List<SqlParameter>
                {
                    new SqlParameter("@partNo", partNo??""),
                    new SqlParameter("@lineId", lineId),
                    new SqlParameter("@statusId", statusId)
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

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            IndexPrep();
            try
            {
                KanbanRequest data = await DbContext.KanbanRequest.Where(a => a.KanbanReqId == id).FirstOrDefaultAsync();
                if (id != 0 && data == null) return NotFound();
                if (id == 0)
                {
                    data = new KanbanRequest();
                    data.RequestDateTime = DateTime.Now;
                    data.StatusId = 1;
                    data.LotNumber = 640;
                    data.TagRequestNo = 0;
                }
                SetViewBag(data);
                //ViewBag.Message = "";
                return View(data);
            }
            catch (Exception e)
            {
                LogHelp.WriteErrorLog(e);
            }
            return View();
        }

        private void SetViewBag(KanbanRequest data)
        {
            if (LoginInfo.RoleId == 2) //user manufacture
            {
                ViewBag.PartFGList = FrmHelp.GetPartFGListByLine((int)LoginInfo.LineId); //hanya dapat menampilkan part FG dari line nya
            }
            else
            {
                ViewBag.PartFGList = FrmHelp.GetPartFGList(); //menampilkan semua part FG
            }
            ViewBag.PartFGName = "";
            ViewBag.LineNo = "";
            ViewBag.LineName = "";
            MsPartFG partFG = DbContext.MsPartFG.Where(a => a.PartFGId == data.PartFGId).FirstOrDefault();
            if (partFG != null)
            {
                ViewBag.PartFGNo = partFG.PartFGNo;
                ViewBag.PartFGName = partFG.PartFGName;
                MsLine line = DbContext.MsLine.Where(a => a.LineId == partFG.LineId).FirstOrDefault();
                if (line != null)
                {
                    ViewBag.LineNo = line.LineNo;
                    ViewBag.LineName = line.LineName;
                }
                ViewBag.Action = "edit";
                ViewBag.KanbanReqId = data.KanbanReqId;
            }
            else
            {
                ViewBag.Action = "add";
            }
        }

        [HttpPost]
        public IActionResult GetPartFGData(int partFGId)
        {
            string partFGName = "";
            string lineNo = "";
            string lineName = "";

            MsPartFG partFG = DbContext.MsPartFG.Where(a => a.PartFGId == partFGId).FirstOrDefault();
            if (partFG != null)
            {
                partFGName = partFG == null ? "" : partFG.PartFGName;
                MsLine line = DbContext.MsLine.Where(a => a.LineId == partFG.LineId).FirstOrDefault();
                if (line != null)
                {
                    lineNo = line.LineNo.ToString();
                    lineName = line.LineName;
                }
            }
            return Json(new { partFGName, lineNo, lineName });
        }

        [HttpPost]
        public string ItemGetData(int kanbanReqId)
        {
            try
            {
                string sp = "sp_KanbanItemGetData";
                List<SqlParameter> param = new List<SqlParameter>
                {
                    new SqlParameter("@kanbanReqId", kanbanReqId)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(KanbanRequest model, string button)
        {
            ViewBag.Message = "";
            if (button == "cancel" && string.IsNullOrEmpty(model.CancelNote))
            {
                ModelState.AddModelError("CancelNote", "The Cancel Note field is required");
            }

            if (ModelState.IsValid)
            {
                switch (button)
                {
                    case "save":
                        model.StatusId = 1;
                        if (model.KanbanReqId == 0)
                        {
                            model.RequestDateTime = DateTime.Now;
                            model.TagRequestNo = GenerateReqNo();
                        }
                        break;
                    case "cancel": model.StatusId = 2; break;
                    case "send": 
                        model.StatusId = 3;
                        model.SendDateTime = DateTime.Now;
                        break;
                    case "receive": 
                        model.StatusId = 5;
                        model.ReceiveDateTime = DateTime.Now;
                        break;
                }

                KanbanRequest data = await DbContext.KanbanRequest.Where(a => a.KanbanReqId == model.KanbanReqId).FirstOrDefaultAsync();
                try
                {
                    if (data == null)
                    {
                        model.InsertDate = DateTime.Now;
                        model.InsertBy = LoginInfo.UserId;
                        DbContext.KanbanRequest.Add(model);
                        DbContext.SaveChanges();
                        GenerateItem(model.KanbanReqId, model.PartFGId, model.LotNumber);
                    }
                    else
                    {
                        data.RequestDateTime = model.RequestDateTime;
                        data.TagRequestNo = model.TagRequestNo;
                        data.StatusId = model.StatusId;
                        data.PartFGId = model.PartFGId;
                        data.LotNumber = model.LotNumber;
                        data.TrolleyNo = model.TrolleyNo;
                        data.SendDateTime = model.SendDateTime;
                        data.ReceiveDateTime = model.ReceiveDateTime;
                        data.EditDate = DateTime.Now;
                        data.EditBy = LoginInfo.UserId;
                        DbContext.KanbanRequest.Update(data);
                        DbContext.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    LogHelp.WriteErrorLog(e);
                }

                switch (button)
                {
                    case "save":
                        ViewBag.Message = "Data has been saved.";
                        break;
                    case "cancel":
                        ViewBag.Message = "Data has been cancelled.";
                        break;
                    case "send":
                        ViewBag.Message = "Kanban Request been sent to WH.";
                        break;
                    case "receive":
                        ViewBag.Message = "Trolley has been received.";
                        break;
                }
                //return RedirectToAction("Detail", "KanbanRequest", new { id = model.KanbanReqId });
            }
            IndexPrep();
            SetViewBag(model);
            return View(model);
        }

        private void GenerateItem(int kanbanReqId, int partFGId, int lotNumber)
        {
            List<MsPartStructure> partStructs = DbContext.MsPartStructure.Where(a => a.PartFGId == partFGId).ToList();
            foreach(MsPartStructure partStruct in partStructs)
            {
                KanbanReqItem item = new KanbanReqItem();
                item.KanbanReqId = kanbanReqId;
                item.PartId = partStruct.PartId;

                double usage = partStruct.Usage;
                double partNum = lotNumber * usage;

                item.EstKanban = 0;
                item.ReqKanban = 0;

                MsPart part = DbContext.MsPart.Where(a => a.PartId == partStruct.PartId).FirstOrDefault();
                if (part != null)
                {
                    item.EstKanban = (int)Math.Ceiling(partNum / part.LotSize);

                    StockWH stock = DbContext.StockWH.Where(a => a.PartNo == part.PartNo).FirstOrDefault();
                    if (stock != null)
                    {
                        item.ReqKanban = partNum < stock.StockQty ? item.EstKanban : (int)Math.Floor(stock.StockQty / part.LotSize);
                    }
                }

                DbContext.KanbanReqItem.Add(item);
                DbContext.SaveChanges();
            }
        }

        private int GenerateReqNo()
        {
            int maxReqNo = 0;
            try
            {
                KanbanRequest data = DbContext.KanbanRequest.Where(a => a.RequestDateTime.Year == DateTime.Today.Year && a.RequestDateTime.Month == DateTime.Today.Month)
                    .OrderByDescending(a=>a.TagRequestNo).FirstOrDefault();
                if (data != null) maxReqNo = data.TagRequestNo;
            }
            catch(Exception e)
            {
                LogHelp.WriteErrorLog(e);
            }
            return maxReqNo + 1;
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            KanbanRequest model = await DbContext.KanbanRequest.Where(a => a.KanbanReqId == id).FirstOrDefaultAsync();
            try
            {
                ViewBag.PartFGNo = "";
                ViewBag.PartFGName = "";
                ViewBag.LineNo = "";
                ViewBag.LineName = "";
                ViewBag.StatusName = "";
                if (model != null)
                {
                    MsPartFG partFG = DbContext.MsPartFG.Where(a => a.PartFGId == model.PartFGId).FirstOrDefault();
                    if (partFG != null)
                    {
                        ViewBag.PartFGNo = partFG.PartFGNo;
                        ViewBag.PartFGName = partFG.PartFGName;
                        MsLine line = DbContext.MsLine.Where(a => a.LineId == partFG.LineId).FirstOrDefault();
                        if(line != null)
                        {
                            ViewBag.LineNo = line.LineNo;
                            ViewBag.LineName = line.LineName;
                        }
                    }
                    ViewBag.StatusName = FrmHelp.GetKanbanStatusName(model.StatusId);
                }
                else return NotFound();
            }
            catch (Exception e)
            {
                LogHelp.WriteErrorLog(e);
            }
            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            bool status = false;
            KanbanRequest data = await DbContext.KanbanRequest.Where(a => a.KanbanReqId == id).FirstOrDefaultAsync();
            if (data != null)
            {
                List<KanbanReqItem> reqItems = await DbContext.KanbanReqItem.Where(a => a.KanbanReqId == data.KanbanReqId).ToListAsync();
                foreach(KanbanReqItem reqItem in reqItems)
                {
                    DbContext.KanbanReqItem.Remove(reqItem);
                    DbContext.SaveChanges();
                }
                DbContext.KanbanRequest.Remove(data);
                DbContext.SaveChanges();
                status = true;
            }
            else return NotFound();
            return Json(new { status });
        }

        public IActionResult Print(int id)
        {
            IndexPrep();
            var dt = new DataTable();
            string sp = "sp_KanbanReport";
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@kanbanReqId", id));
            param.Add(new SqlParameter("@userId", LoginInfo.UserId));
            dt = SqlHelp.ExecQuery(sp, param);
            List<vKanbanRequest> viewlist = SqlHelp.ConvertToList<vKanbanRequest>(dt);

            vKanbanRequest vKanban = new vKanbanRequest();
            EKanbanReport model = new EKanbanReport();
            if (viewlist != null && viewlist.Count != 0)
            {
                vKanban = viewlist[0];
                model.vKanbanRequest = vKanban;

                //item list
                sp = "sp_KanbanReportItem";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@kanbanReqId", id));
                dt = SqlHelp.ExecQuery(sp, param);
                List<EKanbanReportItem> itemlist = SqlHelp.ConvertToList<EKanbanReportItem>(dt);
                model.ItemList = itemlist;
            }
            else return NotFound();

            return View(model);
        }

        [HttpGet]
        public IActionResult ItemDetail(int id, int kanbanReqId)
        {
            try
            {
                KanbanReqItem data = DbContext.KanbanReqItem.Where(a => a.ReqItemId == id).FirstOrDefault();
                if (data == null)
                {
                    data = new KanbanReqItem();
                    data.KanbanReqId = kanbanReqId;
                }
                SetViewBagItem(data);
                return View(data);
            }
            catch (Exception e)
            {
                LogHelp.WriteErrorLog(e);
            }
            return View();
        }

        private void SetViewBagItem(KanbanReqItem data)
        {
            MsPart part = DbContext.MsPart.Where(a => a.PartId == data.PartId).FirstOrDefault();
            if (part != null)
            {
                ViewBag.PartName = part.PartName;
                ViewBag.PartNo = part.PartNo;
                ViewBag.Supplier = part.Supplier;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ItemSave(KanbanReqItem model)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                KanbanReqItem data = DbContext.KanbanReqItem.Where(a => a.ReqItemId == model.ReqItemId).FirstOrDefault();
                data.ReqKanban = model.ReqKanban;
                data.EditDate = DateTime.Now;
                data.EditBy = LoginInfo.UserId;
                DbContext.KanbanReqItem.Update(data);
                DbContext.SaveChanges();
                status = true;
                return Json(new { status });
            }
            else
            {
                SetViewBagItem(model);
                return Json(new { status, html = ViewHelper.RenderRazorViewToString(this, "ItemDetail", model) });
            }
        }

        [HttpGet]
        public IActionResult CancelNote(int id)
        {
            KanbanRequest data = DbContext.KanbanRequest.Where(a => a.KanbanReqId == id).FirstOrDefault();
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSave(KanbanRequest model)
        {
            bool status = false;
            if (string.IsNullOrEmpty(model.CancelNote))
            {
                ModelState.AddModelError("CancelNote", "The Cancel Note field is required");
            }
            //if (ModelState.IsValid)
            else
            {
                KanbanRequest data = await DbContext.KanbanRequest.Where(a => a.KanbanReqId == model.KanbanReqId).FirstOrDefaultAsync();
                try
                {
                    if (data == null) return NotFound();
                    else
                    {
                        data.CancelNote = model.CancelNote;
                        data.StatusId = 2;
                        data.EditDate = DateTime.Now;
                        data.EditBy = LoginInfo.UserId;
                        DbContext.KanbanRequest.Update(data);
                        DbContext.SaveChanges();
                        status = true;
                    }
                }
                catch (Exception e)
                {
                    LogHelp.WriteErrorLog(e);
                }
            }
            IndexPrep();
            return Json(new { status, html = ViewHelper.RenderRazorViewToString(this, "CancelNote", model) });
            //return Json(new { status });
        }
    }
}
