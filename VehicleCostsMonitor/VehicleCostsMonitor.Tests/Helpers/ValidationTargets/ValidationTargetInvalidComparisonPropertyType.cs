namespace VehicleCostsMonitor.Tests.Helpers.ValidationTargets
{
    using VehicleCostsMonitor.Web.Infrastructure.Attributes;

    public class ValidationTargetInvalidComparisonPropertyType
    {
        public string FirstNumber { get; set; }

        [EqualOrGreaterThan(nameof(FirstNumber))]
        public int SecondNumber { get; set; }
    }
}
