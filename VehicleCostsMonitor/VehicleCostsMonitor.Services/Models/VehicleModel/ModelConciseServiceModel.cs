namespace VehicleCostsMonitor.Services.Models.VehicleModel
{
    using VehicleCostsMonitor.Common.AutoMapping;
    using VehicleCostsMonitor.Models;

    public class ModelConciseServiceModel : IAutoMapWith<Model>
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
    }
}
