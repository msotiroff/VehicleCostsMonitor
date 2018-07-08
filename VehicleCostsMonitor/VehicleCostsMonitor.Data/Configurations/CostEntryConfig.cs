using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleCostsMonitor.Models;

namespace VehicleCostsMonitor.Data.Configurations
{
    public class CostEntryConfig : IEntityTypeConfiguration<CostEntry>
    {
        public void Configure(EntityTypeBuilder<CostEntry> builder)
        {
            builder
                .HasOne(ce => ce.Vehicle)
                .WithMany(v => v.CostEntries)
                .HasForeignKey(ce => ce.VehicleId);
        }
    }
}
