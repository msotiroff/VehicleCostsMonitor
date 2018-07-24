namespace VehicleCostsMonitor.Services.Models.Vehicle
{
    using AutoMapper;
    using Common.AutoMapping;
    using VehicleCostsMonitor.Models;

    public class VehicleDetailsServiceModel : IAutoMapWith<Vehicle>, ICustomMappingConfiguration
    {
        public int Id { get; set; }

        public string FullModelName { get; set; }
        
        public int YearOfManufacture { get; set; }
        
        public int EngineHorsePower { get; set; }
        
        public string VehicleTypeName { get; set; }
        
        public string FuelTypeName { get; set; }
        
        public string GearingTypeName { get; set; }
        
        public string OwnerUserName { get; set; }

        public double FuelConsumption { get; set; }
        
        public string PicturePath { get; set; }

        public double TotalFuelAmount { get; set; }
        
        public int TotalDistance { get; set; }

        public decimal TotalFuelCostsPerHundredKm { get; set; }
        
        public decimal TotalFuelCosts { get; set; }
        
        public decimal TotalOtherCosts { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Vehicle, VehicleDetailsServiceModel>()
                .ForMember(dest => dest.FullModelName,
                    opt => opt.MapFrom(src => $"{src.Manufacturer.Name} {src.Model.Name} {src.ExactModelname}"))
                .ForMember(dest => dest.OwnerUserName, 
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.TotalFuelCostsPerHundredKm,
                    opt => opt.MapFrom(src => src.TotalFuelCosts / (src.TotalDistance != 0 ? src.TotalDistance : 100) * 100));
        }
    }
}
