namespace VehicleCostsMonitor.Models
{
    using System.Collections.Generic;

    public class FuelType : BaseType
    {
        public IEnumerable<FuelEntry> FuelEntries { get; set; }
    }
}