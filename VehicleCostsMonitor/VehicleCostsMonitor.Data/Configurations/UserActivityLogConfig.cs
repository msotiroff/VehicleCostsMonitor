namespace VehicleCostsMonitor.Data.Configurations
{
    using VehicleCostsMonitor.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserActivityLogConfig : IEntityTypeConfiguration<UserActivityLog>
    {
        public void Configure(EntityTypeBuilder<UserActivityLog> builder)
        {
            builder
                .HasIndex(l => l.UserEmail);
        }
    }
}
