using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class KanbanRequest
    {
        [Key]
        public int KanbanReqId { get; set; }
        [Required]
        [Display(Name ="Request Date")]
        public DateTime RequestDateTime { get; set; }
        [Display(Name ="Request No")]
        public int TagRequestNo { get; set; }
        [Display(Name ="Status")]
        public byte StatusId { get; set; }
        [Required]
        [Display(Name = "Part FG No")]
        public int PartFGId { get; set; }
        [Required]
        [Display(Name = "Lot Prod. Qty")]
        public int LotNumber { get; set; }
        [Required]
        [Display(Name = "Trolley No")]
        public int TrolleyNo { get; set; }
        [Display(Name = "Send Date")]
        public DateTime? SendDateTime { get; set; }
        [Display(Name = "Return Date")]
        public DateTime? ReturnDateTime { get; set; }
        [Display(Name = "Receive Date")]
        public DateTime? ReceiveDateTime { get; set; }
        [Display(Name = "Picker Name")]
        public string PickerName { get; set; }
        [Display(Name = "Cancel Note")]
        public string CancelNote { get; set; }
        [Display(Name = "Is Printed")]
        //public bool? IsPrinted { get; set; }
        public DateTime? PrintDateTime { get; set; }
        public short? RePrint { get; set; }
        public short? RePrintWH { get; set; }
        public DateTime? InsertDate { get; set; }
        public short? InsertBy { get; set; }
        public DateTime? EditDate { get; set; }
        public short? EditBy { get; set; }
    }
}
