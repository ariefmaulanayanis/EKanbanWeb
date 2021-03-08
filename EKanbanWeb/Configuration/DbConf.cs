using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Configuration
{
    public class DbConf
    {
        public string userId { get; set; }
        public string password { get; set; }

        public IConfiguration Configuration { get; }
        public DbConf(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            if (hostEnvironment.IsDevelopment())
            {
                userId = "sa";
                password = "p@ssw0rd";
            }
            if (hostEnvironment.IsProduction())
            {
                userId = "sa";
                password = "12345";
            }
        }
        public string GetConnectionString()
        {
            string connString = Configuration.GetConnectionString("EKanbanWebDBConnection");
            connString += "User ID=" + userId + ";Password=" + password + ";";
            return connString;
        }
    }
}
