using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.ViewModel
{
    public class MenuViewModel
    {
        [Key]
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public string LinkUrl { get; set; }
        public string Icon { get; set; }
    }
}
