using EKanbanWeb.Data;
using EKanbanWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Helpers
{
    public class FormHelper
    {
        private EKanbanWebDBContext Db { get; set; }
        public FormHelper(EKanbanWebDBContext dbContext)
        {
            Db = dbContext;
        }

        //select list
        public SelectList GetRoleList()
        {
            var lst = Db.SyRole.ToList();
            var sl = lst.Select(a => new SelectListItem
            {
                Text = a.RoleName,
                Value = a.RoleId.ToString()
            });
            return new SelectList(sl, "Value", "Text");
        }

        public SelectList GetLineList()
        {
            var lst = Db.MsLine.ToList();
            var sl = lst.Select(a => new SelectListItem
            {
                Text = a.LineNo + " - " + a.LineName,
                Value = a.LineId.ToString()
            });
            return new SelectList(sl, "Value", "Text");
        }
        public SelectList GetLineList(int id)
        {
            var lst = Db.MsLine.ToList();
            var sl = lst.Select(a => new SelectListItem
            {
                Text = a.LineNo + " - " + a.LineName,
                Value = a.LineId.ToString()
            });
            return new SelectList(sl, "Value", "Text", id);
        }

        public SelectList GetKanbanStatusList()
        {
            var lst = Db.MsKanbanStatus.ToList();
            var sl = lst.Select(a => new SelectListItem
            {
                Text = a.StatusName,
                Value = a.StatusId.ToString()
            });
            return new SelectList(sl, "Value", "Text");
        }
        public SelectList GetKanbanStatusList(byte id)
        {
            var lst = Db.MsKanbanStatus.ToList();
            var sl = lst.Select(a => new SelectListItem
            {
                Text = a.StatusName,
                Value = a.StatusId.ToString()
            });
            return new SelectList(sl, "Value", "Text", id);
        }

        public SelectList GetPartFGList()
        {
            var lst = Db.MsPartFG.ToList();
            var sl = lst.Select(a => new SelectListItem
            {
                Text = a.PartFGNo,
                Value = a.PartFGId.ToString()
            });
            return new SelectList(sl, "Value", "Text");
        }
        public SelectList GetPartFGListByLine(int lineId)
        {
            var lst = Db.MsPartFG.Where(a=>a.LineId==lineId).ToList();
            var sl = lst.Select(a => new SelectListItem
            {
                Text = a.PartFGNo,
                Value = a.PartFGId.ToString()
            });
            return new SelectList(sl, "Value", "Text");
        }

        //reference
        public string GetRoleName(byte id)
        {
            var name = "";
            SyRole data = Db.SyRole.Where(a => a.RoleId == id).FirstOrDefault();
            if (data != null) name = data.RoleName;
            return name;
        }

        public string GetLineName(int? id)
        {
            var name = "-";
            if (id != null)
            {
                MsLine data = Db.MsLine.Where(a => a.LineId == id).FirstOrDefault();
                if (data != null) name = data.LineName;
            }
            return name;
        }

        public string GetPartFGName(int id)
        {
            var name = "-";
            MsPartFG data = Db.MsPartFG.Where(a => a.PartFGId == id).FirstOrDefault();
            if (data != null) name = data.PartFGName;
            return name;
        }

        public string GetKanbanStatusName(byte id)
        {
            var name = "-";
            MsKanbanStatus data = Db.MsKanbanStatus.Where(a => a.StatusId == id).FirstOrDefault();
            if (data != null) name = data.StatusName;
            return name;
        }
    }
}
