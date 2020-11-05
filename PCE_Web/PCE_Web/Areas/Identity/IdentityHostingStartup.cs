using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PCE_Web.Areas.Identity.Data;
using PCE_Web.Data;

[assembly: HostingStartup(typeof(PCE_Web.Areas.Identity.IdentityHostingStartup))]
namespace PCE_Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<DatabaseContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DatabaseContextConnection")));

                services.AddDefaultIdentity<AccountUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<DatabaseContext>();
            });
        }
    }
}