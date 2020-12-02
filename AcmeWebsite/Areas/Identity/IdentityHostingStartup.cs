using System;
using AcmeWebsite.Areas.Identity.Data;
using AcmeWebsite.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AcmeWebsite.Areas.Identity.IdentityHostingStartup))]
namespace AcmeWebsite.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AcmeWebsiteContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AcmeWebsiteContextConnection")));

                services.AddDefaultIdentity<AcmeWebsiteUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<AcmeWebsiteContext>();
            });
        }
    }
}