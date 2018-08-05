namespace VehicleCostsMonitor.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Vehicle;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;

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
        
        public IQueryable<VehicleSearchServiceModel> Get(int manufacturerId, string modelName, string exactModelName, 
            int vehicleTypeId, int fuelTypeId, int gearingTypeId, int engineHorsePowerMin, int engineHorsePowerMax, 
            int yearOfManufactureMin, int yearOfManufactureMax, int minimumKilometers)
        {
            var modelNameSearchSubstring = modelName != GlobalConstants.SearchTermForAllModels ? modelName : "";
            var exaxtModelNameSearchSubstring = string.IsNullOrWhiteSpace(exactModelName) ? "" : exactModelName;
            engineHorsePowerMax = engineHorsePowerMax != default(int) ? engineHorsePowerMax : int.MaxValue;
            yearOfManufactureMax = yearOfManufactureMax != default(int) ? yearOfManufactureMax : int.MaxValue;
            minimumKilometers = minimumKilometers != default(int) ? minimumKilometers : int.MaxValue;

            var vehicles = this.db
                .Vehicles
                .Where(v =>
                    v.ManufacturerId == manufacturerId &&
                    v.Model.Name.Contains(modelNameSearchSubstring) &&
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
                .Where(v => v.Id == id)
                .ProjectTo<VehicleDetailsServiceModel>()
                .FirstOrDefaultAsync();

            return vehicle;
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
