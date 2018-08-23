namespace VehicleCostsMonitor.Common.AutoMapping.Interfaces
{
    using AutoMapper;

    public interface ICustomMappingConfiguration
    {
        void ConfigureMapping(Profile mapper);
    }
}
