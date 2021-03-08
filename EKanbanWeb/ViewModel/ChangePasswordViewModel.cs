using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required]
        //[DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        [StringLength(30)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [StringLength(30)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [StringLength(30)]
        public string ConfirmPassword { get; set; }
    }
}
