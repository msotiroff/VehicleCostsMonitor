namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models
{
    using System.Collections.Generic;

    public class Statistics
    {
        public Dictionary<string, int> Routes { get; set; }

        public Dictionary<string, decimal> Costs { get; set; }

        public List<ConsumptionInRange> ConsumptionRanges { get; set; }

        public double MaxConsumption { get; set; }

        public double MinConsumption { get; set; }
    }
}
