namespace VehicleCostsMonitor.Tests.Helpers.ValidationTargets
{
    using VehicleCostsMonitor.Web.Infrastructure.Attributes;

    public class ValidationTargetInvalidPropertyType
    {
        public int FirstNumber { get; set; }

        [EqualOrGreaterThan(nameof(FirstNumber))]
        public string SecondNumber { get; set; }
    }
}
