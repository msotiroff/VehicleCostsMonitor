namespace VehicleCostsMonitor.Services.Models.Vehicle
{
    using Common.AutoMapping.Interfaces;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;

    public class ModelConciseServiceModel : IAutoMapWith<Model>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string Name { get; set; }

        public int ManufacturerId { get; set; }

        [Display(Name = "Make")]
        public string ManufacturerName { get; set; }
    }
}
