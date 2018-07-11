namespace VehicleCostsMonitor.Services.Interfaces
{
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Services.Models.VehicleModel;

    public interface IVehicleModelService
    {
        Task<bool> CreateAsync(string modelName, int manufacturerId);

        Task<bool> DeleteAsync(int id);

        Task<ModelConciseServiceModel> GetAsync(int id);
    }
}
