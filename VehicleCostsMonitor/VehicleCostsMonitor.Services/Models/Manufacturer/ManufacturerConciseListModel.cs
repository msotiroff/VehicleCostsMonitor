namespace VehicleCostsMonitor.Services.Models.Manufacturer
{
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Common.AutoMapping;
    using VehicleCostsMonitor.Models;

    public class ManufacturerConciseListModel : IAutoMapWith<Manufacturer>
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        [Display(Name = "Models count")]
        public int ModelsCount { get; set; }
    }
}
