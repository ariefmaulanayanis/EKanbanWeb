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
        public int PartNo { get; set; }
        public int StockQty { get; set; }
        public int SyncDate { get; set; }
    }
}
