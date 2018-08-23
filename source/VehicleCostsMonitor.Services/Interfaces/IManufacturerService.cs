namespace VehicleCostsMonitor.Services.Interfaces
{
    using Models.Manufacturer;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IManufacturerService
    {
        Task<ManufacturerUpdateServiceModel> GetForUpdateAsync(int id);

        Task<IEnumerable<ManufacturerConciseListModel>> AllAsync();

        Task<int> CreateAsync(string name);

        Task<bool> UpdateAsync(int id, string name);

        Task<bool> DeleteAsync(int id);

        Task<ManufacturerDetailsServiceModel> GetDetailedAsync(int id);
    }
}
