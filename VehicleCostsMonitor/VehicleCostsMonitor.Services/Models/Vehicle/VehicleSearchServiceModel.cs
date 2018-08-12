namespace VehicleCostsMonitor.Services.Models.Vehicle
{
    using AutoMapper;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Common;
    using VehicleCostsMonitor.Common.AutoMapping.Interfaces;
    using VehicleCostsMonitor.Models;

    public class VehicleSearchServiceModel : IAutoMapWith<Vehicle>, ICustomMappingConfiguration
    {
        public int Id { get; set; }

        [Display(Name = "Make")]
        public string FullModelName { get; set; }

        [Display(Name = "Picture")]
        public string PicturePath { get; set; }

        [Display(Name = "Consumption")]
        public double FuelConsumption { get; set; }

        [Display(Name = "Owner")]
        public string OwnerUserName { get; set; }
        
        public string OwnerId { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Vehicle, VehicleSearchServiceModel>()
                .ForMember(dest => dest.FullModelName,
                    opt => opt.MapFrom(src => $"{src.Manufacturer.Name} {src.Model.Name} {src.ExactModelname}"))
                .ForMember(dest => dest.PicturePath,
                    opt => opt.MapFrom(src => src.Picture != null ? src.Picture.Path : GlobalConstants.DefaultPicturePath))
                .ForMember(dest => dest.OwnerUserName,
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.OwnerId,
                    opt => opt.MapFrom(src => src.User.Id));
        }
    }
}
