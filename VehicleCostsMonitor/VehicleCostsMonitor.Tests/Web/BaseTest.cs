namespace VehicleCostsMonitor.Tests.Web
{
    public abstract class BaseTest
    {
        protected BaseTest()
        {
            TestSetup.InitializeMapper();
        }
    }
}
