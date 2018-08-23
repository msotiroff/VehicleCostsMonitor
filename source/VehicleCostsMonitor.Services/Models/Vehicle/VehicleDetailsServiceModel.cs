namespace VehicleCostsMonitor.Services.Models.Vehicle
{
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Common;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Entries;

    public class VehicleDetailsServiceModel : IAutoMapWith<Vehicle>, ICustomMappingConfiguration
    {
        public int Id { get; set; }

        [Display(Name = "Full model name")]
        public string FullModelName { get; set; }

        [Display(Name = "Yaer")]
        public int YearOfManufacture { get; set; }

        [Display(Name = "Power")]
        public int EngineHorsePower { get; set; }

        [Display(Name = "Vehicle type")]
        public string VehicleTypeName { get; set; }

        [Display(Name = "Fuel type")]
        public string FuelTypeName { get; set; }

        [Display(Name = "Gearing type")]
        public string GearingTypeName { get; set; }

        [Display(Name = "Owner")]
        public string OwnerUserName { get; set; }

        [Display(Name = "Currency")]
        public string OwnerDisplayCurrency { get; set; }

        [Display(Name = "Fuel consumption")]
        public double FuelConsumption { get; set; }

        [Display(Name = "Distance")]
        public int TotalDistance { get; set; }

        [Display(Name = "Total fuel amount")]
        public double TotalFuelAmount { get; set; }

        [Display(Name = "Picture")]
        public string PicturePath { get; set; }

        public ICollection<FuelEntryDetailsModel> FuelEntries { get; set; }

        public ICollection<CostEntryDetailsModel> CostEntries { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<Vehicle, VehicleDetailsServiceModel>()
                .ForMember(dest => dest.FullModelName,
                    opt => opt.MapFrom(src => $"{src.Manufacturer.Name} {src.Model.Name} {src.ExactModelname}"))
                .ForMember(dest => dest.OwnerUserName,
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.OwnerDisplayCurrency,
                    opt => opt.MapFrom(src => src.User.DisplayCurrency.Code))
                .ForMember(dest => dest.PicturePath,
                    opt => opt.MapFrom(src => src.Picture != null ? src.Picture.Path : GlobalConstants.DefaultPicturePath));
        }
    }
}
