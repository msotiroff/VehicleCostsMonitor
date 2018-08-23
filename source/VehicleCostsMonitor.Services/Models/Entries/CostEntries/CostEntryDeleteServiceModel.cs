namespace VehicleCostsMonitor.Services.Models.Entries.CostEntries
{
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
    using System;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;

    public class CostEntryDeleteServiceModel : IAutoMapWith<CostEntry>, ICustomMappingConfiguration
    {
        [Required]
        public int Id { get; set; }
        
        [Display(Name = "Date")]
        public DateTime DateCreated { get; set; }

        public int? Odometer { get; set; }

        [Display(Name = "Type")]
        public string CostEntryTypeName { get; set; }

        public decimal Price { get; set; }

        public string Note { get; set; }

        [Display(Name = "Vehicle")]
        public string VehicleFullName { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<CostEntry, CostEntryDeleteServiceModel>()
                .ForMember(dest => dest.VehicleFullName,
                    opt => opt.MapFrom(src => $"{src.Vehicle.Manufacturer.Name} {src.Vehicle.Model.Name} {src.Vehicle.ExactModelname}"));
        }
    }
}
