using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class MsPartStructure
    {
        [Key]
        public int PartStructId { get; set; }
        public int PartFGId { get; set; }
        public int PartId { get; set; }
        public float Usage { get; set; }
        public string Remarks { get; set; }
        public DateTime? SyncDate { get; set; }
    }
}
