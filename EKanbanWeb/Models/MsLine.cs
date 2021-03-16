using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class MsLine
    {
        [Key]
        public int LineId { get; set; }
        public string LineName { get; set; }
        public short LineNo { get; set; }
        public string LeaderName { get; set; }
        public string SpvName { get; set; }
        public int LeadtimeTrolley { get; set; }
        public DateTime? SyncDate { get; set; }
    }
}
