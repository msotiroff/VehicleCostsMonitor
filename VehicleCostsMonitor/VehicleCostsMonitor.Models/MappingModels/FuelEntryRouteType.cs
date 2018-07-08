namespace VehicleCostsMonitor.Models
{
    public class FuelEntryRouteType
    {
        public int FuelEntryId { get; set; }

        public FuelEntry FuelEntry { get; set; }

        public int RouteTypeId { get; set; }

        public RouteType RouteType { get; set; }
    }
}