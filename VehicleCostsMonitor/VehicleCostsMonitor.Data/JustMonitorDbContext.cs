namespace VehicleCostsMonitor.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using VehicleCostsMonitor.Data.Configurations;
    using VehicleCostsMonitor.Models;

    public class JustMonitorDbContext : IdentityDbContext<User>
    {
        public JustMonitorDbContext(DbContextOptions<JustMonitorDbContext> options)
            : base(options) { }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<FuelEntry> FuelEntries { get; set; }
        public DbSet<CostEntry> CostEntries { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }

        public DbSet<FuelEntryRouteType> FuelEntryRouteTypes { get; set; }
        public DbSet<FuelEntryExtraFuelConsumer> FuelEntryExtraFuelConsumers { get; set; }

        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<FuelEntryType> FuelEntryTypes { get; set; }
        public DbSet<CostEntryType> CostEntryTypes { get; set; }
        public DbSet<ExtraFuelConsumer> ExtraFuelConsumers { get; set; }
        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<GearingType> GearingTypes { get; set; }
        public DbSet<RouteType> RouteTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CostEntryConfig());
            builder.ApplyConfiguration(new FuelEntryConfig());
            builder.ApplyConfiguration(new FuelEntryExtraFuelConsumerConfig());
            builder.ApplyConfiguration(new FuelEntryRouteTypeConfig());
            builder.ApplyConfiguration(new ManufacturerConfig());
            builder.ApplyConfiguration(new ModelConfig());
            builder.ApplyConfiguration(new VehicleConfig());
            builder.ApplyConfiguration(new UserActivityLogConfig());

            base.OnModelCreating(builder);
        }
    }
}
