namespace VehicleCostsMonitor.Services.Models.Entries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
    using Interfaces;
    using VehicleCostsMonitor.Models;

    public class FuelEntryDetailsModel : IEntryModel, IAutoMapWith<FuelEntry>, ICustomMappingConfiguration
    {
        public int Id { get; set; }
        
        public DateTime DateCreated { get; set; }
        
        public int Odometer { get; set; }
        
        public int TripOdometer { get; set; }
        
        public double FuelQuantity { get; set; }

        public string FuelEntryTypeName { get; set; }
        
        public string FuelTypeName { get; set; }

        public decimal Price { get; set; }

        public string CurrencyCode { get; set; }

        public double? Average { get; set; }
        
        public int VehicleId { get; set; }

        public string Owner { get; set; }

        public string Note { get; set; }

        public ICollection<string> Routes { get; set; }

        public override string ToString() => this.FuelEntryTypeName;

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<FuelEntry, FuelEntryDetailsModel>()
                .ForMember(dest => dest.Owner, 
                    opt => opt.MapFrom(src => src.Vehicle.User.UserName))
                .ForMember(dest => dest.Routes, 
                    opt => opt.MapFrom(src => src.Routes.Select(r => r.RouteType.Name)));
        }
    }
}
