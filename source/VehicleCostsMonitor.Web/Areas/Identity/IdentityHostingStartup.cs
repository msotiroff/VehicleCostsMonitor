using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleCostsMonitor.Data;
using VehicleCostsMonitor.Models;

[assembly: HostingStartup(typeof(VehicleCostsMonitor.Web.Areas.Identity.IdentityHostingStartup))]
namespace VehicleCostsMonitor.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {

            });
        }
    }
}