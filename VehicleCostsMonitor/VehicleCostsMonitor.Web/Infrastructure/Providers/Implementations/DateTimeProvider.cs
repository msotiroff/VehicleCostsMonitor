namespace VehicleCostsMonitor.Web.Infrastructure.Providers.Implementations
{
    using System;
    using Interfaces;

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime() => DateTime.UtcNow;
    }
}
