using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleCostsMonitor.Services.Models.Manufacturer;

namespace VehicleCostsMonitor.Services.Interfaces
{
    public interface IManufacturerService
    {
        Task<ManufacturerUpdateServiceModel> GetAsync(int id);

        Task<IEnumerable<ManufacturerConciseListModel>> AllAsync();

        Task<bool> CreateAsync(string name);

        Task<bool> UpdateAsync(int id, string name);

        Task<bool> DeleteAsync(int id);

        Task<ManufacturerDetailsServiceModel> GetDetailedAsync(int id);
    }
}
