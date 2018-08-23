namespace VehicleCostsMonitor.Tests
{
    using AutoMapper;
    using VehicleCostsMonitor.Tests.Helpers;

    public class TestSetup
    {
        private static readonly object sync = new object();
        private static bool mapperInitialized = false;

        public static void InitializeMapper()
        {
            lock (sync)
            {
                if (!mapperInitialized)
                {
                    Mapper.Initialize(config =>
                    {
                        config.AddProfile<TestProfile>();
                    });

                    mapperInitialized = true;
                }
            }
        }
    }
}
