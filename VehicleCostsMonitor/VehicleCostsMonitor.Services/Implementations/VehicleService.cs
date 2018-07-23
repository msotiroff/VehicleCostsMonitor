namespace VehicleCostsMonitor.Services.Implementations
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Interfaces;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Vehicle;

    public class VehicleService : DataAccessService, IVehicleService
    {
        public VehicleService(JustMonitorDbContext db) 
            : base(db) { }

        public async Task<int> CreateAsync(VehicleCreateServiceModel model)
        {
            var newVehicle = Mapper.Map<Vehicle>(model);

            if (!this.ValidateEntityState(newVehicle))
            {
                throw new InvalidOperationException("Entity validation failed!");
            }

            await this.db.AddAsync(newVehicle);
            await this.db.SaveChangesAsync();

            return newVehicle.Id;
        }
    }
}
