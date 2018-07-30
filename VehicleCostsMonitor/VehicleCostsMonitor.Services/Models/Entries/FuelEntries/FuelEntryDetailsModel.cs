namespace VehicleCostsMonitor.Services.Models.Entries
{
    using System;
    using AutoMapper;
    using Common.AutoMapping;
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
        
        public double? Average { get; set; }
        
        public int VehicleId { get; set; }

        public string Owner { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<FuelEntry, FuelEntryDetailsModel>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Vehicle.User.UserName));
        }
    }
}
