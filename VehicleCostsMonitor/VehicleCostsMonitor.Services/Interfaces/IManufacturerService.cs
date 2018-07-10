using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleCostsMonitor.Services.Models.Manufacturer;

namespace VehicleCostsMonitor.Services.Interfaces
{
    public interface IManufacturerService
    {
        Task<ManufacturerUpdateServiceModel> Get(int id);

        Task<IEnumerable<ManufacturerConciseListModel>> All();

        Task<bool> CreateAsync(string name);

        Task<bool> UpdateAsync(int id, string name);

        Task<bool> DeleteAsync(int id);

        Task<ManufacturerDetailsServiceModel> GetDetailed(int id);
    }
}
