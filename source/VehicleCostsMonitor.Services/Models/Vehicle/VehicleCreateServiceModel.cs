namespace VehicleCostsMonitor.Services.Models.Vehicle
{
    using Common.AutoMapping.Interfaces;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;
    using static VehicleCostsMonitor.Models.Common.ModelConstants;

    public class VehicleCreateServiceModel : IAutoMapWith<Vehicle>
    {
        [Required]
        public int ManufacturerId { get; set; }

        [Required]
        public string ModelName { get; set; }

        public string ExactModelname { get; set; }

        [Required]
        public int YearOfManufacture { get; set; }

        [Required]
        [Range(EngineHorsePowerMinValue, EngineHorsePowerMaxValue)]
        public int EngineHorsePower { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        [Required]
        public int FuelTypeId { get; set; }

        [Required]
        public int GearingTypeId { get; set; }

        [Required]
        public string UserId { get; set; }
        
        public int? PictureId { get; set; }
    }
}
