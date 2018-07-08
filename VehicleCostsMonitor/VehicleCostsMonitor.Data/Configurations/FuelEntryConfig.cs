﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleCostsMonitor.Models;

namespace VehicleCostsMonitor.Data.Configurations
{
    public class FuelEntryConfig : IEntityTypeConfiguration<FuelEntry>
    {
        public void Configure(EntityTypeBuilder<FuelEntry> builder)
        {
            builder
                .HasOne(fe => fe.Vehicle)
                .WithMany(v => v.FuelEntries)
                .HasForeignKey(fe => fe.VehicleId);

            builder
                .HasMany(fe => fe.Routes)
                .WithOne(r => r.FuelEntry)
                .HasForeignKey(r => r.FuelEntryId);

            builder
                .HasMany(fe => fe.ExtraFuelConsumers)
                .WithOne(efc => efc.FuelEntry)
                .HasForeignKey(efc => efc.FuelEntryId);
        }
    }
}
