namespace VehicleCostsMonitor.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;

    public interface ICurrencyService
    {
        Task<IEnumerable<Currency>> GetAsync();

        Task<Currency> GetByCodeAsync(string currencyCode);
    }
}
