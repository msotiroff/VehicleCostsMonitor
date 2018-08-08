namespace VehicleCostsMonitor.Services.Interfaces
{
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;

    public interface IDataAccessService
    {
        Task<int> UpdateStatsOnCostEntryChangedAsync(int vehicleId);

        Task<int> UpdateStatsOnFuelEntryChangedAsync(int vehicleId);
    }
}