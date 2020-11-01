using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PCE_Web;
using PCE_Web.Areas.Identity.Data;
using PCE_Web.Classes;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace PCE_Web
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<PCEDatabaseContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("PCEDatabaseContextConnection")));

                services.AddDefaultIdentity<AccountUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<PCEDatabaseContext>();
            });
        }
    }
}