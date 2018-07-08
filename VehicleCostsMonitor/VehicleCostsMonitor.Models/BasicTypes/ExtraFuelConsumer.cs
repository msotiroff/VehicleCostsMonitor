using System.Collections.Generic;

namespace VehicleCostsMonitor.Models
{
    public class ExtraFuelConsumer : BaseType
    {
        public ExtraFuelConsumer()
        {
            this.FuelEntries = new HashSet<FuelEntryExtraFuelConsumer>();
        }

        public IEnumerable<FuelEntryExtraFuelConsumer> FuelEntries { get; set; }
    }
}