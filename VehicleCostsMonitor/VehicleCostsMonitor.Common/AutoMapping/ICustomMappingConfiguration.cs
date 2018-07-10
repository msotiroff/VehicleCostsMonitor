using AutoMapper;

namespace VehicleCostsMonitor.Common.AutoMapping
{
    public interface ICustomMappingConfiguration
    {
        void ConfigureMapping(Profile mapper);
    }
}
