namespace VehicleCostsMonitor.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using VehicleCostsMonitor.Models;

    public class FuelEntryExtraFuelConsumerConfig : IEntityTypeConfiguration<FuelEntryExtraFuelConsumer>
    {
        public void Configure(EntityTypeBuilder<FuelEntryExtraFuelConsumer> builder)
        {
            builder
                .HasKey(x => new { x.FuelEntryId, x.ExtraFuelConsumerId });
        }
    }
}
