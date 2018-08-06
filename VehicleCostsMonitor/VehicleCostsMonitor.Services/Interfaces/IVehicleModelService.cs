namespace VehicleCostsMonitor.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Services.Models.Vehicle;

    public interface IVehicleModelService
    {
        Task<bool> CreateAsync(string modelName, int manufacturerId);

        Task<bool> DeleteAsync(int id);

        Task<ModelConciseServiceModel> GetAsync(int id);

        Task<IEnumerable<string>> GetByManufacturerIdAsync(int manufacturerId);

        Task<bool> UpdateAsync(int id, string name);
    }
}
