using EKanbanWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Data
{
    public class EKanbanWebDBContext:DbContext
    {
        public EKanbanWebDBContext() { }

        public EKanbanWebDBContext(DbContextOptions options) : base(options) { }

        public DbSet<SyRole> SyRole { get; set; }
        public DbSet<SyMenu> SyMenu { get; set; }
        public DbSet<SyRoleAccess> SyRoleAccess { get; set; }
        public DbSet<SyUser> SyUser { get; set; }
        public DbSet<MsLine> MsLine { get; set; }
        public DbSet<MsKanbanStatus> MsKanbanStatus { get; set; }
        public DbSet<MsPartFG> MsPartFG { get; set; }
        public DbSet<MsPart> MsPart { get; set; }
        public DbSet<MsPartStructure> MsPartStructure { get; set; }
        public DbSet<KanbanRequest> KanbanRequest { get; set; }
        public DbSet<KanbanReqItem> KanbanReqItem { get; set; }
    }
}
