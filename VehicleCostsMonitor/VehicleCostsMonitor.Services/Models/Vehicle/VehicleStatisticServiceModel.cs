namespace VehicleCostsMonitor.Services.Models.Vehicle
{
    public class VehicleStatisticServiceModel
    {
        public int ManufacturerId { get; set; }

        public string ManufacturerName { get; set; }

        public string ModelName { get; set; }

        public string FuelType { get; set; }

        public int Count { get; set; }

        public double Average { get; set; }
    }
}
