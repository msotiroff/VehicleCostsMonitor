namespace VehicleCostsMonitor.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Entries.FuelEntries;

    public interface IFuelEntryService
    {
        Task<IEnumerable<FuelEntryType>> GetEntryTypes();

        Task<IEnumerable<RouteType>> GetRouteTypes();

        Task<IEnumerable<ExtraFuelConsumer>> GetExtraFuelConsumers();

        Task<IEnumerable<FuelType>> GetFuelTypes();

        Task<int> GetPreviousOdometerValue(int vehicleId, DateTime currentEntryDate);

        Task<bool> CreateAsync(FuelEntryCreateServiceModel model);

        Task<FuelEntry> GetAsync(int id);

        Task<bool> UpdateAsync(FuelEntry fuelEntry);

        Task<FuelEntryDeleteServiceModel> GetForDeleteAsync(int id);

        Task<bool> DeleteAsync(int id);
    }
}
