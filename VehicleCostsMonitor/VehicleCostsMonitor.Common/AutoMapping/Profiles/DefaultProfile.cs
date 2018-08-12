namespace VehicleCostsMonitor.Common.AutoMapping.Profiles
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Interfaces;

    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            this.ConfigureProfile();
        }

        private void ConfigureProfile()
        {
            var allTypes = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(a => a.GetName().FullName.Contains(nameof(VehicleCostsMonitor)))
                .SelectMany(a => a.GetTypes());
            
            var allMappingTypes = allTypes
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .Select(i => i.GetGenericTypeDefinition())
                            .Contains(typeof(IAutoMapWith<>)))
                .Select(t => new
                {
                    Destination = t,
                    Sourse = t.GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .Select(i => new
                        {
                            Definition = i.GetGenericTypeDefinition(),
                            Arguments = i.GetGenericArguments()
                        })
                        .Where(i => i.Definition == typeof(IAutoMapWith<>))
                        .SelectMany(i => i.Arguments)
                        .First()
                })
                .ToList();

            //Creates bidirectional mapping for all types, which extends IAutoMapWith<TModel> interface
            foreach (var type in allMappingTypes)
            {
                this.CreateMap(type.Destination, type.Sourse);
                this.CreateMap(type.Sourse, type.Destination);
            }

            // Creates custom mapping configuration for all types, which extends ICustomMappingConfiguration interface
            allTypes
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && typeof(ICustomMappingConfiguration).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<ICustomMappingConfiguration>()
                .ToList()
                .ForEach(mapping => mapping.ConfigureMapping(this));
        }
    }
}
