namespace VehicleCostsMonitor.Services.Interfaces
{
    using Models.Vehicle;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Services.Models.Entries.Interfaces;

    public interface IVehicleService
    {
        Task<int> CreateAsync(VehicleCreateServiceModel model);

        Task<VehicleDetailsServiceModel> GetAsync(int id);

        IQueryable<IEntryModel> GetEntries(int vehicleId);
    }
}
