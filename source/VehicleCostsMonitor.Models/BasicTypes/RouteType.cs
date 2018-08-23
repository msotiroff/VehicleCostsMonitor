namespace VehicleCostsMonitor.Models
{
    using System.Collections.Generic;

    public class RouteType : BaseType
    {
        public RouteType()
        {
            this.FuelEntries = new HashSet<FuelEntryRouteType>();
        }

        public IEnumerable<FuelEntryRouteType> FuelEntries { get; set; }
    }
}