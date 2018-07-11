namespace VehicleCostsMonitor.Models
{
    using System.Collections.Generic;

    public class ExtraFuelConsumer : BaseType
    {
        public ExtraFuelConsumer()
        {
            this.FuelEntries = new HashSet<FuelEntryExtraFuelConsumer>();
        }

        public IEnumerable<FuelEntryExtraFuelConsumer> FuelEntries { get; set; }
    }
}