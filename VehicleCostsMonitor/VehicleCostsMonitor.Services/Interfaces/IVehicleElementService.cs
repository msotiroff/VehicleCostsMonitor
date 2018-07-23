namespace VehicleCostsMonitor.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;

    public interface IVehicleElementService
    {
        Task<IEnumerable<VehicleType>> GetVehicleTypes();

        Task<IEnumerable<GearingType>> GetGearingTypes();

        Task<IEnumerable<FuelType>> GetFuelTypes();
    }
}
