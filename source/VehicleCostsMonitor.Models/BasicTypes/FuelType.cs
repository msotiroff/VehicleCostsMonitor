namespace VehicleCostsMonitor.Models
{
    using System.Collections.Generic;

    public class FuelType : BaseType
    {
        public FuelType()
        {
            this.FuelEntries = new HashSet<FuelEntry>();
        }

        public IEnumerable<FuelEntry> FuelEntries { get; set; }
    }
}