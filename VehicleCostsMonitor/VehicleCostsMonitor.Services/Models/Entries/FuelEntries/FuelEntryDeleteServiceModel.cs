namespace VehicleCostsMonitor.Services.Models.Entries.FuelEntries
{
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
    using System;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;

    public class FuelEntryDeleteServiceModel : IAutoMapWith<FuelEntry>, ICustomMappingConfiguration
    {
        [Required]
        public int Id { get; set; }
        
        [Display(Name = "Date")]
        public DateTime DateCreated { get; set; }
        
        public int Odometer { get; set; }

        [Display(Name = "Quantity")]
        public double FuelQuantity { get; set; }
        
        public decimal Price { get; set; }

        public string Note { get; set; }

        [Display(Name = "Fuel type")]
        public string FuelTypeName { get; set; }

        [Display(Name = "Type")]
        public string FuelEntryTypeName { get; set; }
        
        public double Average { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Display(Name = "Vehicle")]
        public string VehicleFullName { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<FuelEntry, FuelEntryDeleteServiceModel>()
                .ForMember(dest => dest.VehicleFullName,
                    opt => opt.MapFrom(src => $"{src.Vehicle.Manufacturer.Name} {src.Vehicle.Model.Name} {src.Vehicle.ExactModelname}"));
        }
    }
}
