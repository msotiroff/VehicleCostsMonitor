namespace VehicleCostsMonitor.Web.Infrastructure.Extensions
{
    using AutoMapper;
    using Common;
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Models.Dtos;
    using Newtonsoft.Json;
    using Services.Models.Entries.FuelEntries;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using static VehicleCostsMonitor.Models.Common.ModelConstants;

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

                SeedRequiredData(dbContext);

                // Comment this row if you don't need users and vehicles initial seed!
                SeedOptionalData(serviceScope, dbContext);
            }

            return app;
        }

        private static void SeedOptionalData(IServiceScope serviceScope, JustMonitorDbContext dbContext)
        {
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            SeedDefaultRoles(userManager, roleManager, dbContext);
            SeedUsers(dbContext, userManager);
            SeedVehicles(dbContext);
            SeedFuelEntries(dbContext);
            SeedCostEntries(dbContext);
        }

        private static void SeedRequiredData(JustMonitorDbContext dbContext)
        {
            SeedManufacturersWithModels(dbContext);
            SeedCostEntryTypes(dbContext);
            SeedExtraFuelConsumers(dbContext);
            SeedFuelEntryTypes(dbContext);
            SeedFuelTypes(dbContext);
            SeedGearingTypes(dbContext);
            SeedRouteTypes(dbContext);
            SeedVehicleTypes(dbContext);
            SeedCurrencies(dbContext);
        }

        private static void UpdateVehiclesStats(JustMonitorDbContext dbContext)
        {
            var firstFuelingTypeId = dbContext.FuelEntryTypes.First(fet => fet.Name == "First fueling").Id;

            var vehicles = dbContext
                .Vehicles
                .Include(v => v.CostEntries)
                .Include(v => v.FuelEntries)
                .ToList();

            foreach (var vehicle in vehicles)
            {
                var fuelSumWithoutFirstFueling = vehicle.FuelEntries.Where(fe => fe.FuelEntryTypeId != firstFuelingTypeId).Sum(fe => fe.FuelQuantity);

                vehicle.TotalDistance = vehicle.FuelEntries.Sum(fe => fe.TripOdometer);
                vehicle.TotalFuelAmount = vehicle.FuelEntries.Sum(fe => fe.FuelQuantity);
                vehicle.FuelConsumption = fuelSumWithoutFirstFueling / vehicle.TotalDistance * 100.0;
            }

            dbContext.Vehicles.UpdateRange(vehicles);
            dbContext.SaveChanges();
        }

        private static void SeedCurrencies(JustMonitorDbContext dbContext)
        {
            if (!dbContext.Currencies.Any())
            {
                var currenciesAsJson = File.ReadAllText(WebConstants.CurrenciesListPath);
                var currencies = JsonConvert.DeserializeObject<Currency[]>(currenciesAsJson);

                dbContext.AddRange(currencies);
                dbContext.SaveChanges();
            }
        }

        private static void SeedCostEntries(JustMonitorDbContext dbContext)
        {
            if (!dbContext.CostEntries.Any())
            {
                var allVehicles = dbContext.Vehicles.ToList();

                var defaultCurrencyId = dbContext
                    .Currencies
                    .First(c => c.Code == GlobalConstants.DefaultCurrencyCode)
                    .Id;

                var random = new Random();

                var currentDate = DateTime.UtcNow;
                var minDay = 1;
                var maxDay = 29;

                var costEntryTypesIds = dbContext.CostEntryTypes.Select(cet => cet.Id).ToArray();

                var note = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
                var costEntries = new List<CostEntry>();


                foreach (var vehicle in allVehicles)
                {
                    var startDate = new DateTime(currentDate.Year - 2, 1, 1);

                    while (startDate < currentDate)
                    {
                        var day = random.Next(minDay, maxDay);
                        var entryDate = new DateTime(startDate.Year, startDate.Month, day);
                        var costEntryTypeId = costEntryTypesIds[random.Next(0, costEntryTypesIds.Length)];
                        var price = (decimal)random.NextDouble() * 250;

                        var entry = new CostEntry(entryDate, costEntryTypeId, vehicle.Id, price, defaultCurrencyId, note);
                        costEntries.Add(entry);

                        startDate = startDate.AddMonths(4);
                    }
                }

                dbContext.CostEntries.AddRange(costEntries);
                dbContext.SaveChanges();
            }
        }

        private static void SeedFuelEntries(JustMonitorDbContext dbContext)
        {
            if (!dbContext.FuelEntries.Any())
            {
                var allVehicles = dbContext.Vehicles.ToList();

                var defaultCurrencyId = dbContext
                    .Currencies
                    .First(c => c.Code == GlobalConstants.DefaultCurrencyCode)
                    .Id;

                var random = new Random();

                var currentDate = DateTime.UtcNow;
                var minDay = 1;
                var maxDay = 29;

                var odometerMinStep = 250;
                var odometerMaxStep = 750;

                var fuelQuantityMinValue = 20;
                var fuelQuantityMaxValue = 80;

                var note = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

                var firstFuelingTypeId = dbContext.FuelEntryTypes.First(fet => fet.Name == "First fueling").Id;
                var fullFuelingTypeId = dbContext.FuelEntryTypes.First(fet => fet.Name == "Full").Id;

                var routesIds = dbContext.RouteTypes.Select(r => r.Id).ToArray();

                var extrasIds = dbContext.ExtraFuelConsumers.Select(ex => ex.Id).ToArray();

                var fuelEntriesToBeCreated = new List<FuelEntry>();

                foreach (var vehicle in allVehicles)
                {
                    var odometer = 100000;
                    var startDate = new DateTime(currentDate.Year - 2, 1, 1);
                    var hasAnyEntries = false;

                    while (startDate < currentDate)
                    {
                        var day = random.Next(minDay, maxDay);
                        var fuelingDate = new DateTime(startDate.Year, startDate.Month, day);
                        var quantity = random.Next(fuelQuantityMinValue, fuelQuantityMaxValue);
                        var tripOdometer = random.Next(odometerMinStep, odometerMaxStep);
                        if (hasAnyEntries)
                        {
                            odometer += tripOdometer;
                        }
                        
                        var model = new FuelEntryCreateServiceModel
                        {
                            DateCreated = fuelingDate,
                            Odometer = odometer,
                            TripOdometer = hasAnyEntries ? tripOdometer : 0,
                            Average = hasAnyEntries ? ((double)quantity / tripOdometer) * 100 : 0,
                            FuelQuantity = quantity,
                            Price = (decimal)(quantity * (random.NextDouble() + 0.7)),
                            CurrencyId = defaultCurrencyId,
                            Note = note,
                            FuelEntryTypeId = hasAnyEntries ? fullFuelingTypeId : firstFuelingTypeId,
                            Routes = new List<FuelEntryRouteType>
                            {
                                new FuelEntryRouteType { RouteTypeId = routesIds[random.Next(0, routesIds.Length)]}
                            },
                            ExtraFuelConsumers = new List<FuelEntryExtraFuelConsumer>
                            {
                                new FuelEntryExtraFuelConsumer { ExtraFuelConsumerId = extrasIds[random.Next(0, extrasIds.Length)]}
                            },
                            VehicleId = vehicle.Id,
                            FuelTypeId = vehicle.FuelTypeId
                        };

                        var newFuelEntry = Mapper.Map<FuelEntry>(model);
                        fuelEntriesToBeCreated.Add(newFuelEntry);

                        startDate = startDate.AddMonths(1);
                        hasAnyEntries = true;
                    }
                }

                dbContext.FuelEntries.AddRange(fuelEntriesToBeCreated);
                dbContext.SaveChanges();

                UpdateVehiclesStats(dbContext);
            }
        }

        private static void SeedVehicles(JustMonitorDbContext dbContext)
        {
            if (!dbContext.Vehicles.Any())
            {
                var vehiclesToSeed = new HashSet<Vehicle>();

                var random = new Random();
                var usersIds = dbContext.Users.Select(u => u.Id).ToList();
                var manufacturers = dbContext.Manufacturers.Include(m => m.Models).ToList();
                var fuelTypesIds = dbContext.FuelTypes.Select(ft => ft.Id).ToList();
                var gearingTypesIds = dbContext.GearingTypes.Select(gt => gt.Id).ToList();
                var vehicleTypesIds = dbContext.VehicleTypes.Select(vt => vt.Id).ToList();

                foreach (var userId in usersIds)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        var manufacturer = manufacturers[random.Next(0, manufacturers.Count)];
                        var model = manufacturer.Models.ToList()[random.Next(0, manufacturer.Models.Count())];

                        var vehicle = new Vehicle
                        {
                            UserId = userId,
                            ManufacturerId = manufacturer.Id,
                            ModelId = model.Id,
                            YearOfManufacture = random.Next(YearOfManufactureMinValue, DateTime.UtcNow.Year),
                            EngineHorsePower = random.Next(60, 151),
                            FuelTypeId = fuelTypesIds[random.Next(0, fuelTypesIds.Count)],
                            GearingTypeId = gearingTypesIds[random.Next(0, gearingTypesIds.Count)],
                            VehicleTypeId = vehicleTypesIds[random.Next(0, vehicleTypesIds.Count)],
                        };

                        vehiclesToSeed.Add(vehicle);
                    }
                }

                dbContext.Vehicles.AddRange(vehiclesToSeed);
                dbContext.SaveChanges();
            }
        }

        private static void SeedUsers(JustMonitorDbContext dbContext, UserManager<User> userManager)
        {
            if (dbContext.Users.Count() <= 1)
            {
                var defaultDisplayCurrencyId = dbContext
                    .Currencies
                    .FirstOrDefault(c => c.Code == GlobalConstants.DefaultCurrencyCode)
                    ?.Id;

                var usersList = File.ReadAllText(WebConstants.UsersListPath);

                var users = JsonConvert.DeserializeObject<User[]>(usersList);

                Task.Run(async () =>
                {
                    foreach (var user in users)
                    {
                        user.EmailConfirmed = true;
                        user.CurrencyId = defaultDisplayCurrencyId;

                        await userManager.CreateAsync(user, "password");
                    }
                })
                .GetAwaiter()
                .GetResult();
            }
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

        private static void SeedManufacturersWithModels(JustMonitorDbContext dbContext)
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

        private static void SeedDefaultRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, JustMonitorDbContext dbContext)
        {
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

                    await RegisterAdminUser(userManager, adminRoleName, dbContext);
                })
                .Wait();
        }

        private static async Task RegisterAdminUser(UserManager<User> userManager, string adminRoleName, JustMonitorDbContext dbContext)
        {
            var adminEmail = WebConstants.AdminEmail;
            var adminUserName = WebConstants.AdminUserName;

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var defaultDisplayCurrencyId = dbContext
                    .Currencies
                    .FirstOrDefault(c => c.Code == GlobalConstants.DefaultCurrencyCode)
                    ?.Id;

                adminUser = new User
                {
                    Email = adminEmail,
                    UserName = adminUserName,
                    CurrencyId = defaultDisplayCurrencyId,
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(adminUser, WebConstants.AdminPassword);

                await userManager.AddToRoleAsync(adminUser, adminRoleName);
            }
        }
    }
}
