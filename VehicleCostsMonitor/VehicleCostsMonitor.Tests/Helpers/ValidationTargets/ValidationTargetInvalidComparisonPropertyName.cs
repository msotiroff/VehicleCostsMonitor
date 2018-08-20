using VehicleCostsMonitor.Web.Infrastructure.Attributes;

namespace VehicleCostsMonitor.Tests.Helpers.ValidationTargets
{
    public class ValidationTargetInvalidComparisonPropertyName
    {
        public int FirstNumber { get; set; }

        [EqualOrGreaterThan("Invalid")]
        public int SecondNumber { get; set; }
    }
}
