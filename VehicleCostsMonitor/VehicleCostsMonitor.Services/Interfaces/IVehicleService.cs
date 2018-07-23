namespace VehicleCostsMonitor.Services.Interfaces
{
    using Models.Vehicle;
    using System.Threading.Tasks;

    public interface IVehicleService
    {
        Task<int> CreateAsync(VehicleCreateServiceModel model);
    }
}
