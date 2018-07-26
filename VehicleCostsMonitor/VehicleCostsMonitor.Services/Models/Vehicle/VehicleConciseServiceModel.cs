namespace VehicleCostsMonitor.Services.Models.Vehicle
{
    using AutoMapper;
    using Common.AutoMapping;
    using VehicleCostsMonitor.Common;
    using VehicleCostsMonitor.Models;

    public class VehicleConciseServiceModel : IAutoMapWith<Vehicle>, ICustomMappingConfiguration
    {
        public int Id { get; set; }

        public string FullModelName { get; set; }
        
        public string PicturePath { get; set; }

        public double FuelConsumption { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Vehicle, VehicleConciseServiceModel>()
                .ForMember(dest => dest.FullModelName,
                    opt => opt.MapFrom(src => $"{src.Manufacturer.Name} {src.Model.Name} {src.ExactModelname}"))
                .ForMember(dest => dest.PicturePath, 
                    opt => opt.MapFrom(src => src.Picture != null ? src.Picture.Path : GlobalConstants.DefaultPicturePath));
        }
    }
}
