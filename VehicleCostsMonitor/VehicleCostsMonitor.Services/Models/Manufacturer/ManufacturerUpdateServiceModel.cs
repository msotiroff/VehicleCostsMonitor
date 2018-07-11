namespace VehicleCostsMonitor.Services.Models.Manufacturer
{
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Common.AutoMapping;
    using VehicleCostsMonitor.Models;

    public class ManufacturerUpdateServiceModel : IAutoMapWith<Manufacturer>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
