namespace VehicleCostsMonitor.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using VehicleCostsMonitor.Models;

    public class FuelEntryRouteTypeConfig : IEntityTypeConfiguration<FuelEntryRouteType>
    {
        public void Configure(EntityTypeBuilder<FuelEntryRouteType> builder)
        {
            builder
                .HasKey(x => new { x.FuelEntryId, x.RouteTypeId });
        }
    }
}
