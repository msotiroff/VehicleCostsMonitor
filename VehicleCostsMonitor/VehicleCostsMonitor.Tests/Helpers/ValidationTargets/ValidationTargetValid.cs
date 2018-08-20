using VehicleCostsMonitor.Web.Infrastructure.Attributes;

namespace VehicleCostsMonitor.Tests.Helpers.ValidationTargets
{
    public class ValidationTargetValid
    {
        public int FirstNumber { get; set; }

        [EqualOrGreaterThan(nameof(FirstNumber))]
        public int SecondNumber { get; set; }
    }
}
