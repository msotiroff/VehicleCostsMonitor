namespace VehicleCostsMonitor.Services.Models.User
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Vehicle;

    public class UserProfileServiceModel : IAutoMapWith<User>, ICustomMappingConfiguration
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public IEnumerable<VehicleConciseServiceModel> Vehicles { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<User, UserProfileServiceModel>()
                .ForMember(dest => dest.Vehicles, 
                    opt => opt.MapFrom(src => src.Vehicles.Where(v => !v.IsDeleted)));
        }
    }
}
