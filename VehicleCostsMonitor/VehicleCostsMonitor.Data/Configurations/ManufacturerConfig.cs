﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleCostsMonitor.Models;

namespace VehicleCostsMonitor.Data.Configurations
{
    public class ManufacturerConfig : IEntityTypeConfiguration<Manufacturer>
    {
        public void Configure(EntityTypeBuilder<Manufacturer> builder)
        {
            builder
                .HasIndex(m => m.Name)
                .IsUnique(true);
        }
    }
}
