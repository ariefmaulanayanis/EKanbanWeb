using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class StockWH
    {
        [Key]
        public int StockId { get; set; }
        public string PartNo { get; set; }
        public double StockQty { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
