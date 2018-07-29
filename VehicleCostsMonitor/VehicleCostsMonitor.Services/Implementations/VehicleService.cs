﻿namespace VehicleCostsMonitor.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Entries.Interfaces;
    using Models.Vehicle;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Entries;

    public class VehicleService : DataAccessService, IVehicleService
    {
        public VehicleService(JustMonitorDbContext db)
            : base(db) { }

        public async Task<int> CreateAsync(VehicleCreateServiceModel model)
        {
            var newVehicle = Mapper.Map<Vehicle>(model);
            newVehicle.Model = await this.db.Models
                .FirstOrDefaultAsync(m => m.ManufacturerId == model.ManufacturerId && m.Name == model.ModelName);

            this.ValidateEntityState(newVehicle);

            await this.db.AddAsync(newVehicle);
            await this.db.SaveChangesAsync();

            return newVehicle.Id;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehicle = await this.db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return false;
            }

            vehicle.IsDeleted = true;
            this.db.Update(vehicle);
            await this.db.SaveChangesAsync();

            return true;
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
            var fuelEntries = this.db.FuelEntries.Where(fe => fe.VehicleId == vehicleId).ProjectTo<FuelEntryDetailsModel>().Cast<IEntryModel>();
            var costEntries = this.db.CostEntries.Where(fe => fe.VehicleId == vehicleId).ProjectTo<CostEntryDetailsModel>().Cast<IEntryModel>();
            var allEntries = new List<IEntryModel>(fuelEntries);
            allEntries.AddRange(costEntries);

            return allEntries.OrderByDescending(e => e.DateCreated).AsQueryable();
        }

        public async Task<VehicleUpdateServiceModel> GetForUpdateAsync(int id)
            => await this.db
                .Vehicles
                .Where(v => v.Id == id)
                .ProjectTo<VehicleUpdateServiceModel>()
                .FirstOrDefaultAsync();

        public async Task<bool> UpdateAsync(VehicleUpdateServiceModel model)
        {
            try
            {
                var vehicle = Mapper.Map<Vehicle>(model);
                var dbModel = await this.db
                        .Models
                        .FirstOrDefaultAsync(m => m.ManufacturerId == model.ManufacturerId && m.Name == model.ModelName);

                vehicle.ModelId = dbModel.Id;

                this.db.Update(vehicle);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
