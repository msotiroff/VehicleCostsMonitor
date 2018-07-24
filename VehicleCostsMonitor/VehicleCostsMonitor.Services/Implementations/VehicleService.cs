﻿namespace VehicleCostsMonitor.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Entries.Interfaces;
    using Models.Vehicle;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;

    public class VehicleService : DataAccessService, IVehicleService
    {
        public VehicleService(JustMonitorDbContext db) 
            : base(db) { }

        public async Task<int> CreateAsync(VehicleCreateServiceModel model)
        {
            var newVehicle = Mapper.Map<Vehicle>(model);
            newVehicle.Model = await this.db
                .Models
                .FirstOrDefaultAsync(m => m.ManufacturerId == model.ManufacturerId && m.Name == model.ModelName);

            this.ValidateEntityState(newVehicle);

            await this.db.AddAsync(newVehicle);
            await this.db.SaveChangesAsync();

            return newVehicle.Id;
        }

        public async Task<VehicleDetailsServiceModel> GetAsync(int id)
        {
            var vehicle = await this.db
                .Vehicles
                .Where(v => v.Id == id)
                .ProjectTo<VehicleDetailsServiceModel>()
                .FirstOrDefaultAsync();

            return vehicle;
        }

        public IQueryable<IEntryModel> GetEntries(int vehicleId)
        {
            var fuelEntries = this.db.FuelEntries.Where(fe => fe.VehicleId == vehicleId).Cast<IEntryModel>();
            var costEntries = this.db.CostEntries.Where(fe => fe.VehicleId == vehicleId).Cast<IEntryModel>();
            var allEntries = fuelEntries.Concat(costEntries).OrderByDescending(e => e.DateCreated);

            return allEntries;
        }
    }
}
