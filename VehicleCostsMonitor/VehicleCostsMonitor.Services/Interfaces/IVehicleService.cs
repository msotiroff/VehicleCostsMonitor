namespace VehicleCostsMonitor.Services.Interfaces
{
    using Models.Vehicle;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IVehicleService
    {
        Task<int> CreateAsync(VehicleCreateServiceModel model);

        Task<VehicleDetailsServiceModel> GetAsync(int id);

        Task<VehicleUpdateServiceModel> GetForUpdateAsync(int id);

        Task<bool> UpdateAsync(VehicleUpdateServiceModel model);

        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<VehicleStatisticServiceModel>> GetMostEconomicCars();

        IQueryable<VehicleSearchServiceModel> Get(
            int manufacturerId, string modelName, string exactModelName, int vehicleTypeId, 
            int fuelTypeId, int gearingTypeId, int engineHorsePowerMin, int engineHorsePowerMax, 
            int yearOfManufactureMin, int yearOfManufactureMax, int minimumKilometers);
    }
}
