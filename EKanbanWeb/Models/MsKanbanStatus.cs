using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class MsKanbanStatus
    {
        [Key]
        public byte StatusId { get; set; }
        public string StatusName { get; set; }
    }
}
