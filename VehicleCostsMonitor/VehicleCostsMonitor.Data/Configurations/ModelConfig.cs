using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleCostsMonitor.Models;

namespace VehicleCostsMonitor.Data.Configurations
{
    public class ModelConfig : IEntityTypeConfiguration<Model>
    {
        public void Configure(EntityTypeBuilder<Model> builder)
        {
            builder
                .HasOne(m => m.Manufacturer)
                .WithMany(man => man.Models)
                .HasForeignKey(m => m.ManufactureId);
        }
    }
}
