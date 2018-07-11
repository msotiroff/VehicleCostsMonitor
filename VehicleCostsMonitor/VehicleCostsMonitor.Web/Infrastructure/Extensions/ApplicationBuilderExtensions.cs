namespace VehicleCostsMonitor.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Models.Dtos;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;

    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// This is a workaround for missing seed data functionality in EF-7
        /// </summary>
        /// <param name="app">
        /// An instance that provides the mechanisms to get instance of the database context.
        /// </param>
        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<JustMonitorDbContext>();
                dbContext.Database.Migrate();

                SeedDefaultRoles(serviceScope);

                SeedDefaultManufacturers(dbContext);

                SeedCostEntryTypes(dbContext);
                SeedExtraFuelConsumers(dbContext);
                SeedFuelEntryTypes(dbContext);
                SeedFuelTypes(dbContext);
                SeedGearingTypes(dbContext);
                SeedRouteTypes(dbContext);
                SeedVehicleTypes(dbContext);
            }

            return app;
        }

        private static void SeedVehicleTypes(JustMonitorDbContext dbContext)
        {
            if (!dbContext.VehicleTypes.Any())
            {
                var vehicleTypesList = File
                    .ReadAllText(WebConstants.VehicleTypesListPath);

                var vehicleTypes = JsonConvert.DeserializeObject<VehicleType[]>(vehicleTypesList);

                dbContext.VehicleTypes.AddRange(vehicleTypes);
                dbContext.SaveChanges();
            }
        }

        private static void SeedRouteTypes(JustMonitorDbContext dbContext)
        {
            if (!dbContext.RouteTypes.Any())
            {
                var routeTypesList = File
                    .ReadAllText(WebConstants.RouteTypesListPath);

                var routeTypes = JsonConvert.DeserializeObject<RouteType[]>(routeTypesList);

                dbContext.RouteTypes.AddRange(routeTypes);
                dbContext.SaveChanges();
            }
        }

        private static void SeedGearingTypes(JustMonitorDbContext dbContext)
        {
            if (!dbContext.GearingTypes.Any())
            {
                var gearingTypesList = File
                    .ReadAllText(WebConstants.GearingTypesListPath);

                var gearingTypes = JsonConvert.DeserializeObject<GearingType[]>(gearingTypesList);

                dbContext.GearingTypes.AddRange(gearingTypes);
                dbContext.SaveChanges();
            }
        }

        private static void SeedFuelTypes(JustMonitorDbContext dbContext)
        {
            if (!dbContext.FuelTypes.Any())
            {
                var fuelTypesList = File
                    .ReadAllText(WebConstants.FuelTypesListPath);

                var fuelTypes = JsonConvert.DeserializeObject<FuelType[]>(fuelTypesList);

                dbContext.FuelTypes.AddRange(fuelTypes);
                dbContext.SaveChanges();
            }
        }

        private static void SeedFuelEntryTypes(JustMonitorDbContext dbContext)
        {
            if (!dbContext.FuelEntryTypes.Any())
            {
                var fuelEntryTypesList = File
                    .ReadAllText(WebConstants.FuelEntryTypesListPath);

                var fuelEntryTypes = JsonConvert.DeserializeObject<FuelEntryType[]>(fuelEntryTypesList);

                dbContext.FuelEntryTypes.AddRange(fuelEntryTypes);
                dbContext.SaveChanges();
            }
        }

        private static void SeedExtraFuelConsumers(JustMonitorDbContext dbContext)
        {
            if (!dbContext.ExtraFuelConsumers.Any())
            {
                var extraFuelConsumersList = File
                    .ReadAllText(WebConstants.ExtraFuelConsumersListPath);
                
                var extraFuelConsumers = JsonConvert.DeserializeObject<ExtraFuelConsumer[]>(extraFuelConsumersList);

                dbContext.ExtraFuelConsumers.AddRange(extraFuelConsumers);
                dbContext.SaveChanges();
            }
        }

        private static void SeedCostEntryTypes(JustMonitorDbContext dbContext)
        {
            if (!dbContext.CostEntryTypes.Any())
            {
                var costEntryTypesList = File
                    .ReadAllText(WebConstants.CostEntryTypesListPath);

                var costEntryTypes = JsonConvert.DeserializeObject<CostEntryType[]>(costEntryTypesList);

                dbContext.CostEntryTypes.AddRange(costEntryTypes);
                dbContext.SaveChanges();
            }
        }

        private static void SeedDefaultManufacturers(JustMonitorDbContext dbContext)
        {
            if (!dbContext.Manufacturers.Any())
            {
                var manufacturersList = File
                    .ReadAllText(WebConstants.ManufacturersListPath);

                var manufacturers = JsonConvert.DeserializeObject<ManufacturerDto[]>(manufacturersList);

                var manufacturersToSeed = new List<Manufacturer>();

                foreach (var manufacturer in manufacturers)
                {
                    var dbManufacturer = new Manufacturer
                    {
                        Name = manufacturer.Name,
                        Models = manufacturer
                            .Models
                            .Select(modelName => new Model { Name = modelName })
                            .ToList()
                    };

                    manufacturersToSeed.Add(dbManufacturer);
                }

                dbContext.Manufacturers.AddRange(manufacturersToSeed);
                dbContext.SaveChanges();
            }
        }

        private static void SeedDefaultRoles(IServiceScope serviceScope)
        {
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            Task
                .Run(async () =>
                {
                    var adminRoleName = WebConstants.AdministratorRole;
                    var roles = new[]
                    {
                        adminRoleName
                    };

                    foreach (var role in roles)
                    {
                        var roleExist = await roleManager.RoleExistsAsync(role);

                        if (!roleExist)
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    await RegisterAdminUser(userManager, adminRoleName);
                })
                .Wait();
        }

        private static async Task RegisterAdminUser(UserManager<User> userManager, string adminRoleName)
        {
            var adminEmail = WebConstants.AdminEmail;
            var adminUserName = WebConstants.AdminUserName;

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Email = adminEmail,
                    UserName = adminUserName
                };

                await userManager.CreateAsync(adminUser, WebConstants.AdminPassword);

                await userManager.AddToRoleAsync(adminUser, adminRoleName);
            }
        }
    }
}
