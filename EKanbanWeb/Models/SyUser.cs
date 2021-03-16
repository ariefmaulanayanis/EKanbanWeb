using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models
{
    public class SyUser
    {
        [Key]
        public short UserId { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Username")]
        public string UserName { get; set; }
        [Required]
        [StringLength(30)]
        public string Password { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Real Name")]
        public string RealName { get; set; }
        [Required]
        [DisplayName("Role")]
        public byte RoleId { get; set; }
        [DisplayName("Line")]
        public int? LineId { get; set; }
        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public DateTime? InsertDate { get; set; }
        public short? InsertBy { get; set; }
        public DateTime? EditDate { get; set; }
        public short? EditBy { get; set; }
    }
}
