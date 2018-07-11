namespace VehicleCostsMonitor.Common.AutoMapping
{
    using AutoMapper;

    public interface ICustomMappingConfiguration
    {
        void ConfigureMapping(Profile mapper);
    }
}
