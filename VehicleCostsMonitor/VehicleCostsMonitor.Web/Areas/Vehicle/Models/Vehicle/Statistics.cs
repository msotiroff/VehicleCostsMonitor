namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models
{
    using System.Collections.Generic;

    public class Statistics
    {
        public Dictionary<string, int> Routes { get; set; }

        public Dictionary<string, decimal> Costs { get; set; }
    }
}
