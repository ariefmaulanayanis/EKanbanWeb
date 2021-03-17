using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class MsPart
    {
        [Key]
        public int PartId { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public double LotSize { get; set; }
        public string Supplier { get; set; }
        public string Zone { get; set; }
        public string Remarks { get; set; }
        public DateTime? SyncDate { get; set; }
    }
}
