namespace VehicleCostsMonitor.Services.Models.VehicleModel
{
    using Common.AutoMapping;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;

    public class ModelConciseServiceModel : IAutoMapWith<Model>
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
