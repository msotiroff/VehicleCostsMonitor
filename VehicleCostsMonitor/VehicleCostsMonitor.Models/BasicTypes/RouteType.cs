using System.Collections.Generic;

namespace VehicleCostsMonitor.Models
{
    public class RouteType : BaseType
    {
        public RouteType()
        {
            this.FuelEntries = new HashSet<FuelEntryRouteType>();
        }

        public IEnumerable<FuelEntryRouteType> FuelEntries { get; set; }
    }
}