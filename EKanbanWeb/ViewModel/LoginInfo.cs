using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.ViewModel
{
    public class LoginInfo
    {
        public Int16 UserId { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public short RoleId { get; set; }
        public string RoleName { get; set; }
        public int? LineId { get; set; }
        public string LineName { get; set; }
    }
}
