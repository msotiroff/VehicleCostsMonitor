using System.Threading.Tasks;
using VehicleCostsMonitor.Services.Models.VehicleModel;

namespace VehicleCostsMonitor.Services.Interfaces
{
    public interface IVehicleModelService
    {
        Task<bool> Create(string modelName, int manufacturerId);

        Task<bool> Delete(int id);

        Task<ModelConciseServiceModel> Get(int id);
    }
}
