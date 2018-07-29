namespace VehicleCostsMonitor.Services.Models.Entries
{
    using AutoMapper;
    using Common.AutoMapping;
    using Interfaces;
    using System;
    using VehicleCostsMonitor.Models;

    public class CostEntryDetailsModel : IEntryModel, IAutoMapWith<CostEntry>, ICustomMappingConfiguration
    {
        public int Id { get; set; }
        
        public DateTime DateCreated { get; set; }

        public int? Odometer { get; set; }
        
        public string CostEntryTypeName { get; set; }

        public decimal Price { get; set; }
        
        public int VehicleId { get; set; }

        public string Owner { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<CostEntry, CostEntryDetailsModel>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Vehicle.User.UserName));
        }
    }
}
