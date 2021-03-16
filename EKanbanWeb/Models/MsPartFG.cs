using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class MsPartFG
    {
        [Key]
        public int PartFGId { get; set; }
        public string PartFGNo { get; set; }
        public string PartFGName { get; set; }
        public int LineId { get; set; }
        public string Remarks { get; set; }
        public DateTime? SyncDate { get; set; }
    }
}
