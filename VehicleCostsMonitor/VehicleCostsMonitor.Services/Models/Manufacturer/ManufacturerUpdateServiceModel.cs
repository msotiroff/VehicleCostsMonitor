namespace VehicleCostsMonitor.Services.Models.Manufacturer
{
    using VehicleCostsMonitor.Common.AutoMapping;
    using VehicleCostsMonitor.Models;

    public class ManufacturerUpdateServiceModel : IAutoMapWith<Manufacturer>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
