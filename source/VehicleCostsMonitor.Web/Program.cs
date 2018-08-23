namespace VehicleCostsMonitor.Web
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) => 
            {
                config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("secrets.json", optional: false, reloadOnChange: true);
            })
            .UseStartup<Startup>();
    }
}
