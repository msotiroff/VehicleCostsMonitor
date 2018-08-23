namespace VehicleCostsMonitor.Tests.Services
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using VehicleCostsMonitor.Data;

    public abstract class BaseTest
    {
        protected BaseTest()
        {
            TestSetup.InitializeMapper();
        }

        protected JustMonitorDbContext DatabaseInstance
        {
            get
            {
                var options = new DbContextOptionsBuilder<JustMonitorDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .EnableSensitiveDataLogging()
               .Options;

                return new JustMonitorDbContext(options);
            }
        }
    }
}
