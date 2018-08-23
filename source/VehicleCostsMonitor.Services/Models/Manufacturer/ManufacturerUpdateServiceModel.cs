namespace VehicleCostsMonitor.Services.Models.Manufacturer
{
    using Common.AutoMapping.Interfaces;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;

    public class ManufacturerUpdateServiceModel : IAutoMapWith<Manufacturer>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
