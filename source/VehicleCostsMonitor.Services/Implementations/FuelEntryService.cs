namespace VehicleCostsMonitor.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Entries.FuelEntries;

    public class FuelEntryService : BaseService, IFuelEntryService
    {
        public FuelEntryService(JustMonitorDbContext db)
            : base(db) { }

        public async Task<bool> CreateAsync(FuelEntryCreateServiceModel model)
        {
            if (model == null)
            {
                return false;
            }

            var modelOdometer = model.Odometer;
            var fuelEntry = Mapper.Map<FuelEntry>(model);

            var vehicle = await this.db
                .Vehicles
                .Where(v => v.Id == model.VehicleId && !v.IsDeleted)
                .AsNoTracking()
                .Include(v => v.FuelEntries)
                .FirstOrDefaultAsync();

            if (vehicle == null)
            {
                return false;
            }

            await this.SetAverageConsumption(modelOdometer, fuelEntry, vehicle);

            try
            {
                this.ValidateEntityState(fuelEntry);

                await this.db.FuelEntries.AddAsync(fuelEntry);
                await this.db.SaveChangesAsync();
                await this.UpdateStatsOnFuelEntryChangedAsync(fuelEntry.VehicleId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<FuelEntry> GetAsync(int id)
            => this.db.FuelEntries
            .Where(fe => fe.Id == id)
            .Include(fe => fe.Routes)
            .Include(fe => fe.ExtraFuelConsumers)
            .FirstOrDefaultAsync();

        public async Task<IEnumerable<FuelEntryType>> GetEntryTypes()
            => await this.db.FuelEntryTypes.ToListAsync();

        public async Task<IEnumerable<ExtraFuelConsumer>> GetExtraFuelConsumers()
            => await this.db.ExtraFuelConsumers.ToListAsync();
        
        public async Task<IEnumerable<FuelType>> GetFuelTypes()
            => await this.db.FuelTypes.ToListAsync();

        public async Task<IEnumerable<RouteType>> GetRouteTypes()
            => await this.db.RouteTypes.ToListAsync();

        public async Task<FuelEntryDeleteServiceModel> GetForDeleteAsync(int id)
            => await this.db.FuelEntries.Where(fe => fe.Id == id).ProjectTo<FuelEntryDeleteServiceModel>().FirstOrDefaultAsync();

        public async Task<int> GetPreviousOdometerValue(int vehicleId, DateTime currentEntryDate)
        {
            var vehicle = await this.db.Vehicles
                .Where(v => v.Id == vehicleId && !v.IsDeleted && v.FuelEntries.Any())
                .Include(v => v.FuelEntries)
                .FirstOrDefaultAsync();

            if (vehicle == null)
            {
                return default(int);
            }


            var fuelEntries = vehicle.FuelEntries.OrderBy(fe => fe.DateCreated);

            var lastOdometer = fuelEntries.LastOrDefault(fe => fe.DateCreated < currentEntryDate)?.Odometer;
            
            return lastOdometer ?? default(int);
        }

        public async Task<bool> UpdateAsync(FuelEntry fuelEntry)
        {
            if (fuelEntry == null)
            {
                return false;
            }

            var vehicle = await this.db
                .Vehicles
                .Where(v => v.Id == fuelEntry.VehicleId && !v.IsDeleted)
                .AsNoTracking()
                .Include(v => v.FuelEntries)
                .FirstOrDefaultAsync();

            if (vehicle == null)
            {
                return false;
            }

            await this.RemoveOldMappingEntities(fuelEntry);

            await this.SetAverageConsumption(fuelEntry.Odometer, fuelEntry, vehicle);

            try
            {
                this.ValidateEntityState(fuelEntry);

                this.db.FuelEntries.Update(fuelEntry);
                await this.db.SaveChangesAsync();
                await this.UpdateStatsOnFuelEntryChangedAsync(fuelEntry.VehicleId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var fuelEntry = await this.db.FuelEntries.FirstOrDefaultAsync(fe => fe.Id == id);
            if (fuelEntry == null)
            {
                return false;
            }

            try
            {
                this.db.Remove(fuelEntry);
                await this.db.SaveChangesAsync();
                await this.UpdateStatsOnFuelEntryChangedAsync(fuelEntry.VehicleId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<int> UpdateStatsOnFuelEntryChangedAsync(int vehicleId)
        {
            var vehicle = await this.db.Vehicles
                .Where(v => v.Id == vehicleId && !v.IsDeleted)
                .Include(v => v.FuelEntries)
                .ThenInclude(fe => fe.FuelEntryType)
                .FirstOrDefaultAsync();

            var fuelEntries = vehicle.FuelEntries.OrderBy(fe => fe.DateCreated).ToArray();

            vehicle.TotalDistance = fuelEntries.Sum(fe => fe.TripOdometer);
            vehicle.TotalFuelAmount = fuelEntries.Sum(fe => fe.FuelQuantity);

            var lastFullFuelEntry = fuelEntries.LastOrDefault(fe => fe.FuelEntryType.Name == FullFueling);
            var indexOfLastFullFueling = Math.Max(0, Array.LastIndexOf(fuelEntries, lastFullFuelEntry));

            var reducedCollection = fuelEntries.Take(indexOfLastFullFueling + 1).ToArray();
            var firstFullFuelEntry = reducedCollection.FirstOrDefault(fe => fe.FuelEntryType.Name == FullFueling);
            if (firstFullFuelEntry == null)
            {
                firstFullFuelEntry = reducedCollection.FirstOrDefault(fe => fe.FuelEntryType.Name == FirstFueling);
            }
            var indexOfFirstFullFueling = Math.Max(0, Array.IndexOf(reducedCollection, firstFullFuelEntry));

            var validFuelEntries = fuelEntries
                .Skip(indexOfFirstFullFueling)
                .Take(indexOfLastFullFueling)
                .ToList();

            var quantitiesSum = validFuelEntries.Sum(fe => fe.FuelQuantity);
            var distance = validFuelEntries.Sum(fe => fe.TripOdometer);

            if (distance != 0)
            {
                vehicle.FuelConsumption = quantitiesSum / distance * 100.0;
            }
            else if (fuelEntries.Count() <= 1)
            {
                vehicle.FuelConsumption = 0;
            }

            this.db.Update(vehicle);
            var affectedRows = await this.db.SaveChangesAsync();

            return affectedRows;
        }

        private async Task RemoveOldMappingEntities(FuelEntry fuelEntry)
        {
            var dbEntryRoutes = await this.db.FuelEntryRouteTypes.Where(fert => fert.FuelEntryId == fuelEntry.Id).ToListAsync();
            var dbEntryExtras = await this.db.FuelEntryExtraFuelConsumers.Where(feex => feex.FuelEntryId == fuelEntry.Id).ToListAsync();
            this.db.RemoveRange(dbEntryRoutes);
            this.db.RemoveRange(dbEntryExtras);
            await this.db.SaveChangesAsync();
        }

        private async Task SetAverageConsumption(int modelOdometer, FuelEntry fuelEntry, Vehicle vehicle)
        {
            var firstFuelingType = await this.db.FuelEntryTypes.FirstAsync(fet => fet.Name == FirstFueling);
            var fullType = await this.db.FuelEntryTypes.FirstAsync(fet => fet.Name == FullFueling);
            var fuelEntries = vehicle.FuelEntries.Where(fe => fe.DateCreated < fuelEntry.DateCreated).OrderBy(fe => fe.DateCreated);

            if (fuelEntries.Any())
            {
                var lastOdometer = fuelEntries.Last().Odometer;
                fuelEntry.TripOdometer = modelOdometer - lastOdometer;
                if (fuelEntry.FuelEntryTypeId == firstFuelingType.Id)
                {
                    fuelEntry.FuelEntryTypeId = fullType.Id;
                }

                if (fuelEntry.FuelEntryTypeId == fullType.Id)
                {
                    var lastFullFueling = fuelEntries.LastOrDefault(fe => fe.FuelEntryTypeId == fullType.Id);
                    if (lastFullFueling == null)
                    {
                        lastFullFueling = fuelEntries.LastOrDefault(fe => fe.FuelEntryTypeId == firstFuelingType.Id);
                    }

                    var distance = fuelEntry.Odometer - lastFullFueling?.Odometer;

                    var indexOfLastFullFueling = Array.LastIndexOf(fuelEntries.OrderBy(fe => fe.DateCreated).ToArray(), lastFullFueling);
                    var sumOfValidFuelQuantities = fuelEntries.Skip(indexOfLastFullFueling + 1).Sum(fe => fe.FuelQuantity) + fuelEntry.FuelQuantity;
                    if (distance != null)
                    {
                        fuelEntry.Average = (sumOfValidFuelQuantities / distance.Value) * 100.0;
                    }
                }
            }
            else
            {
                fuelEntry.FuelEntryTypeId = firstFuelingType.Id;
            }
        }
    }
}
