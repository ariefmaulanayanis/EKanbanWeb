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
        [StringLength(25)]
        [DisplayName("Username")]
        public string UserName { get; set; }
        public string Password { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Real Name")]
        public string RealName { get; set; }
        [Required]
        [DisplayName("Role")]
        public byte RoleId { get; set; }
        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public DateTime? InsertDate { get; set; }
        public short? InsertBy { get; set; }
        public DateTime? EditDate { get; set; }
        public short? EditBy { get; set; }
    }
}
