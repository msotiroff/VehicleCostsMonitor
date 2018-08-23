namespace VehicleCostsMonitor.Services.Implementations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;

    public class VehicleElementService : BaseService, IVehicleElementService
    {
        public VehicleElementService(JustMonitorDbContext db) 
            : base(db) { }

        public async Task<IEnumerable<FuelType>> GetFuelTypes()
            => await this.db.FuelTypes.ToListAsync();

        public async Task<IEnumerable<GearingType>> GetGearingTypes()
            => await this.db.GearingTypes.ToListAsync();

        public async Task<IEnumerable<VehicleType>> GetVehicleTypes()
            => await this.db.VehicleTypes.ToListAsync();
    }
}
