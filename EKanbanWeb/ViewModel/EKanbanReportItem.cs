using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.ViewModel
{
    public class EKanbanReportItem
    {
        [Key]
        public int ReqItemId { get; set; }
        public int KanbanReqId { get; set; }
        public int RowNumber { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public int ReqKanban { get; set; }
        public double LotSize { get; set; }
        public string Zone { get; set; }
        public string Supplier { get; set; }
    }
}
