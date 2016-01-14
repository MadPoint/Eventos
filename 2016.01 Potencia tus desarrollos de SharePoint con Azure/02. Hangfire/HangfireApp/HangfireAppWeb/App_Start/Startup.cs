using Hangfire;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(HangfireAppWeb.App_Start.Startup))]
namespace HangfireAppWeb.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireDB");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
