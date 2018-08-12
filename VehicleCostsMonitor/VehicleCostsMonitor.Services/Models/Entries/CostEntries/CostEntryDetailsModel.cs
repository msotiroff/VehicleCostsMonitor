namespace VehicleCostsMonitor.Services.Models.Entries
{
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
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

        public string CurrencyCode { get; set; }

        public string Note { get; set; }

        public int VehicleId { get; set; }

        public string Owner { get; set; }

        public override string ToString() => this.CostEntryTypeName;

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<CostEntry, CostEntryDetailsModel>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Vehicle.User.UserName));
        }
    }
}
