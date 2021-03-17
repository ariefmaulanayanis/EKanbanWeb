using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class KanbanReqItem
    {
        [Key]
        public int ReqItemId { get; set; }
        public int KanbanReqId { get; set; }
        public int PartId { get; set; }
        public int EstKanban { get; set; }
        [Required]
        public int ReqKanban { get; set; }
        public string Remarks { get; set; }
        public DateTime? InsertDate { get; set; }
        public short? InsertBy { get; set; }
        public DateTime? EditDate { get; set; }
        public short? EditBy { get; set; }
    }
}
