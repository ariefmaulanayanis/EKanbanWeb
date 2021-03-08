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

        public DbSet<SyUser> SyUser { get; set; }
        public DbSet<SyRole> SyRole { get; set; }
    }
}
