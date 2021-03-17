using EKanbanWeb.Models.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.ViewModel
{
    public class EKanbanReport
    {
        //[Key]
        //public int KanbanReqId { get; set; }
        //public DateTime RequestDateTime { get; set; }
        //public string LeaderName { get; set; }
        //public string LineName { get; set; }
        //public short LineNo { get; set; }
        //public string PartFGNo { get; set; }
        //public int LotNumber { get; set; }
        //public int TrolleyNo { get; set; }
        //public string QRCode { get; set; }
        //public int TagRequestNo { get; set; }
        public vKanbanRequest vKanbanRequest { get; set; }
        public List<EKanbanReportItem> ItemList { get; set; }
    }
}
