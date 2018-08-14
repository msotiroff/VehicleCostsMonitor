namespace VehicleCostsMonitor.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Vehicle;
    using Services.Models.Entries;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;

    public class VehicleService : BaseService, IVehicleService
    {
        public VehicleService(JustMonitorDbContext db)
            : base(db) { }

        public async Task<int> CreateAsync(VehicleCreateServiceModel model)
        {
            var newVehicle = Mapper.Map<Vehicle>(model);
            newVehicle.Model = await this.db.Models
                .FirstOrDefaultAsync(m => m.ManufacturerId == model.ManufacturerId && m.Name == model.ModelName);

            try
            {
                this.ValidateEntityState(newVehicle);

                await this.db.AddAsync(newVehicle);
                await this.db.SaveChangesAsync();
            }
            catch
            {
                return default(int);
            }

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
        
        public IQueryable<VehicleSearchServiceModel> Get(int manufacturerId, string modelName, string exactModelName, 
            int vehicleTypeId, int fuelTypeId, int gearingTypeId, int engineHorsePowerMin, int engineHorsePowerMax, 
            int yearOfManufactureMin, int yearOfManufactureMax, int minimumKilometers)
        {
            var modelNameSearchSubstring = modelName != null 
                ? modelName != GlobalConstants.SearchTermForAllModels 
                    ? modelName 
                    : "" 
                    : "";
            
            var exaxtModelNameSearchSubstring = string.IsNullOrWhiteSpace(exactModelName) ? "" : exactModelName;
            engineHorsePowerMax = engineHorsePowerMax != default(int) ? engineHorsePowerMax : int.MaxValue;
            yearOfManufactureMax = yearOfManufactureMax != default(int) ? yearOfManufactureMax : int.MaxValue;

            var vehicles = this.db
                .Vehicles
                .Where(v =>
                    !v.IsDeleted &&
                    v.ManufacturerId == manufacturerId &&
                    v.Model.Name.ToLower().Contains(modelNameSearchSubstring.ToLower()) &&
                    v.ExactModelname.Contains(exaxtModelNameSearchSubstring) &&
                    v.EngineHorsePower >= engineHorsePowerMin &&
                    v.EngineHorsePower <= engineHorsePowerMax &&
                    v.YearOfManufacture >= yearOfManufactureMin &&
                    v.YearOfManufacture <= yearOfManufactureMax &&
                    v.TotalDistance >= minimumKilometers);

            if (vehicleTypeId != default(int))
            {
                vehicles = vehicles.Where(v => v.VehicleTypeId == vehicleTypeId);
            }
            if (fuelTypeId != default(int))
            {
                vehicles = vehicles.Where(v => v.FuelTypeId == fuelTypeId);
            }
            if (gearingTypeId != default(int))
            {
                vehicles = vehicles.Where(v => v.GearingTypeId == gearingTypeId);
            }

            return vehicles
                .ProjectTo<VehicleSearchServiceModel>()
                .OrderBy(v => v.FuelConsumption);
        }

        public async Task<VehicleDetailsServiceModel> GetAsync(int id)
        {
            var vehicle = await this.db
                .Vehicles
                .Where(v => v.Id == id && !v.IsDeleted)
                .ProjectTo<VehicleDetailsServiceModel>()
                .FirstOrDefaultAsync();

            return vehicle;
        }

        public async Task<VehicleUpdateServiceModel> GetForUpdateAsync(int id)
            => await this.db
                .Vehicles
                .Where(v => v.Id == id && !v.IsDeleted)
                .ProjectTo<VehicleUpdateServiceModel>()
                .FirstOrDefaultAsync();

        public async Task<IQueryable<CostEntryDetailsModel>> GetCostEntries(int id)
        {
            var costs = await this.db.CostEntries
            .Where(ce => ce.VehicleId == id && !ce.Vehicle.IsDeleted)
            .OrderByDescending(ce => ce.DateCreated)
            .ProjectTo<CostEntryDetailsModel>()
            .ToListAsync();

            return costs.AsQueryable();
        }

        public async Task<IQueryable<FuelEntryDetailsModel>> GetFuelEntries(int id)
        {
            var fuelings = await this.db.FuelEntries
            .Where(fe => fe.VehicleId == id && !fe.Vehicle.IsDeleted)
            .OrderByDescending(fe => fe.DateCreated)
            .ProjectTo<FuelEntryDetailsModel>()
            .ToListAsync();

            return fuelings.AsQueryable();
        }

        public async Task<IEnumerable<VehicleStatisticServiceModel>> GetMostEconomicCars(string fuelType)
        {
            var vehicles = await this.db.Vehicles
                .Where(v => v.FuelType.Name.ToLower() == fuelType && !v.IsDeleted)
                .GroupBy(v => v.ModelId)
                .OrderBy(vg => vg.Sum(v => v.FuelConsumption) / vg.Count())
                .Take(GlobalConstants.MostEconomicVehiclesListCount)
                .Select(vl => new VehicleStatisticServiceModel
                {
                    ManufacturerId = vl.First().ManufacturerId,
                    ManufacturerName = vl.First().Manufacturer.Name,
                    ModelName = vl.First().Model.Name,
                    FuelType = fuelType,
                    Average = vl.Sum(v => v.FuelConsumption) / vl.Count(),
                    Count = vl.Count()
                })
                .ToListAsync();
            
            return vehicles;
        }

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
