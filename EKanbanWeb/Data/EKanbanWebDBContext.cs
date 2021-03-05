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
    }
}
