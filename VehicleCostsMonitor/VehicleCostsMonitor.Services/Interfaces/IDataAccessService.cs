namespace VehicleCostsMonitor.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IDataAccessService
    {
        Task<int> UpdateStatsOnCostEntryChangedAsync(int vehicleId);

        Task<int> UpdateStatsOnFuelEntryChangedAsync(int vehicleId);
    }
}