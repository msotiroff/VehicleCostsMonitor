namespace VehicleCostsMonitor.Web
{
    using AutoMapper;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Net.Http;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Web.Areas.Identity.Services.Email;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<SendGridOptions>(this.Configuration.GetSection("EmailSetting"))
                .Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            services
                .AddDbContext<JustMonitorDbContext>(options =>
                    options.UseSqlServer(
                        this.Configuration.GetConnectionString("JustMonitor")));

            services.AddTransient<HttpClient>();

            services.AddAuthentication()
                .AddFacebook(options =>
                {
                    options.AppId = this.Configuration.GetSection("ExternalAuthentications:Facebook:AppId").Value;
                    options.AppSecret = this.Configuration.GetSection("ExternalAuthentications:Facebook:AppSecret").Value;
                })
                .AddGoogle(options =>
                {
                    options.ClientId = this.Configuration.GetSection("ExternalAuthentications:Google:ClientId").Value;
                    options.ClientSecret = this.Configuration.GetSection("ExternalAuthentications:Google:ClientSecret").Value;
                });
            
            services
                .AddIdentity<User, IdentityRole>(options =>
                {
                    options.Stores.MaxLengthForKeys = 128;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<JustMonitorDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services
                .AddRouting(options => options.LowercaseUrls = true);

            services.AddDistributedMemoryCache();

            services
                .AddMvc(optiions =>
                {
                    optiions.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services
                .AddApplicationServices()
                .AddDomainServices()
                .AddAutoMapper();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                         name: "areaRoute",
                         template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            app.SeedData();
        }
    }
}
