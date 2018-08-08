namespace VehicleCostsMonitor.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Interfaces;

    public class CurrencyService : DataAccessService, ICurrencyService
    {
        public CurrencyService(JustMonitorDbContext db) 
            : base(db) { }

        public async Task<IEnumerable<Currency>> GetAsync() => await this.db.Currencies.ToListAsync();
    }
}
