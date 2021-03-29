using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models.api
{
    public class KanbanItem
    {
        [Key]
        public int ReqItemId { get; set; }
        public int KanbanReqId { get; set; }
        public DateTime RequestDate { get; set; }
        public int ReqNo { get; set; }
        public int PartId { get; set; }
        public string PartNo { get; set; }
        public double LotSize { get; set; }
        public string Zone { get; set; }
        public int? OrderQty { get; set; }
    }
}
