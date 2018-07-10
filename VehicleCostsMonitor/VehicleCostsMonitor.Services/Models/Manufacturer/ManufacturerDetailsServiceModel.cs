namespace VehicleCostsMonitor.Services.Models.Manufacturer
{
    using System.Collections.Generic;
    using VehicleCostsMonitor.Common.AutoMapping;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.VehicleModel;

    public class ManufacturerDetailsServiceModel : IAutoMapWith<Manufacturer>
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public IEnumerable<ModelConciseServiceModel> Models { get; set; }
    }
}
