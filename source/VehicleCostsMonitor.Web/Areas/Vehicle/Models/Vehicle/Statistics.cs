namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models
{
    using System.Collections.Generic;

    public class Statistics
    {
        public IDictionary<string, int> Routes { get; set; }

        public IDictionary<string, decimal> Costs { get; set; }

        public IList<ConsumptionInRange> ConsumptionRanges { get; set; }

        public IEnumerable<MileageByDate> MileageByDate { get; set; }

        public double MaxConsumption { get; set; }

        public double MinConsumption { get; set; }
    }
}
