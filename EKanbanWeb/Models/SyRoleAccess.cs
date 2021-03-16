using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class SyRoleAccess
    {
        [Key]
        public int RoleAccessId { get; set; }
        public short MenuId { get; set; }
        public byte RoleId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
        public bool CanExport { get; set; }
    }
}
