using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleCostsMonitor.Models;

namespace VehicleCostsMonitor.Data.Configurations
{
    public class FuelEntryExtraFuelConsumerConfig : IEntityTypeConfiguration<FuelEntryExtraFuelConsumer>
    {
        public void Configure(EntityTypeBuilder<FuelEntryExtraFuelConsumer> builder)
        {
            builder
                .HasKey(x => new { x.FuelEntryId, x.ExtraFuelConsumerId });
        }
    }
}
