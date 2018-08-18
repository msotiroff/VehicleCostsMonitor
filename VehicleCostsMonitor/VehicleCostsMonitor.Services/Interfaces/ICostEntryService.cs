namespace VehicleCostsMonitor.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Entries.CostEntries;

    public interface ICostEntryService
    {
        Task<IEnumerable<CostEntryType>> GetEntryTypesAsync();

        Task<CostEntryUpdateServiceModel> GetForUpdateAsync(int id);

        Task<CostEntryDeleteServiceModel> GetForDeleteAsync(int id);
        
        Task<bool> CreateAsync(DateTime dateCreated, int costEntryTypeId, int vehicleId, decimal price, int currencyId, string note, int? odometer);
        
        Task<bool> UpdateAsync(int id, DateTime dateCreated, int costEntryTypeId, decimal price, int currencyId, string note, int? odometer);

        Task<bool> DeleteAsync(int id);
    }
}
