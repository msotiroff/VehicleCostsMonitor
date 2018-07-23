namespace VehicleCostsMonitor.Services.Models.Vehicle
{
    using AutoMapper;
    using Common.AutoMapping;
    using System.Linq;
    using VehicleCostsMonitor.Models;

    public class VehicleConciseServiceModel : IAutoMapWith<Vehicle>, ICustomMappingConfiguration
    {
        public int Id { get; set; }

        public string MakeModel { get; set; }

        public string ExactModelname { get; set; }
        
        public string PicturePath { get; set; }

        public double FuelConsumption { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Vehicle, VehicleConciseServiceModel>()
                .ForMember(dest => dest.FuelConsumption,
                    opt => opt.MapFrom(src => (src.FuelEntries.Sum(fe => fe.Average)) / src.FuelEntries.Count()))
                .ForMember(dest => dest.MakeModel, opt => opt.MapFrom(src => $"{src.Manufacturer.Name} {src.Model.Name}"));
        }
    }
}
