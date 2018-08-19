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
            if (model == null)
            {
                return default(int);
            }

            var manufacturerExist = await this.db.Manufacturers.AnyAsync(m => m.Id == model.ManufacturerId);
            if (!manufacturerExist)
            {
                return default(int);
            }

            var newVehicle = Mapper.Map<Vehicle>(model);

            newVehicle.Model = await this.db.Models
                .FirstOrDefaultAsync(m => m.ManufacturerId == model.ManufacturerId && m.Name == model.ModelName);

            if (newVehicle.Model == null)
            {
                return default(int);
            }

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
            
            var exaxtModelNameSearchSubstring = string.IsNullOrWhiteSpace(exactModelName) ? "" : exactModelName.ToLower();
            engineHorsePowerMax = engineHorsePowerMax != default(int) ? engineHorsePowerMax : int.MaxValue;
            yearOfManufactureMax = yearOfManufactureMax != default(int) ? yearOfManufactureMax : int.MaxValue;

            var vehicles = this.db
                .Vehicles
                .Where(v =>
                    !v.IsDeleted &&
                    v.ManufacturerId == manufacturerId &&
                    v.Model.Name.ToLower().Contains(modelNameSearchSubstring.ToLower()) &&
                    (string.IsNullOrWhiteSpace(v.ExactModelname) ||
                        v.ExactModelname.ToLower().Contains(exaxtModelNameSearchSubstring)) &&
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
            => await this.db
            .Vehicles
            .Where(v => v.Id == id && !v.IsDeleted)
            .ProjectTo<VehicleDetailsServiceModel>()
            .FirstOrDefaultAsync();

        public async Task<VehicleUpdateServiceModel> GetForUpdateAsync(int id)
            => await this.db.Vehicles.Where(v => v.Id == id).ProjectTo<VehicleUpdateServiceModel>().FirstOrDefaultAsync();

        public IQueryable<CostEntryDetailsModel> GetCostEntries(int id)
        {
            var costs = this.db.CostEntries
            .Where(ce => ce.VehicleId == id && !ce.Vehicle.IsDeleted)
            .OrderByDescending(ce => ce.DateCreated)
            .ProjectTo<CostEntryDetailsModel>();

            return costs;
        }

        public IQueryable<FuelEntryDetailsModel> GetFuelEntries(int id)
        {
            var fuelings = this.db.FuelEntries
            .Where(fe => fe.VehicleId == id && !fe.Vehicle.IsDeleted)
            .OrderByDescending(fe => fe.DateCreated)
            .ProjectTo<FuelEntryDetailsModel>();

            return fuelings;
        }

        public async Task<IEnumerable<VehicleStatisticServiceModel>> GetMostEconomicCars(string fuelType)
        {
            var vehicles = await this.db.Vehicles
                .Where(v => v.FuelType.Name.ToLower() == fuelType.ToLower() && !v.IsDeleted)
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
            if (model == null)
            {
                return false;
            }

            try
            {
                var vehicle = Mapper.Map<Vehicle>(model);
                var dbModel = await this.db
                        .Models
                        .FirstOrDefaultAsync(m => m.ManufacturerId == model.ManufacturerId && m.Name == model.ModelName);

                vehicle.ModelId = dbModel.Id;

                this.ValidateEntityState(vehicle);

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
