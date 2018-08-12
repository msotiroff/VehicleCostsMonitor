namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models.FuelEntry
{
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
    using Infrastructure.Extensions.ExcelExport.Attributes;
    using Services.Models.Entries;
    using System;

    public class FuelEntryExcelViewModel : IAutoMapWith<FuelEntryDetailsModel>, ICustomMappingConfiguration
    {
        [ExcelDisplayName("Date")]
        [ExcelValueFormat("dd-MM-yyyy")]
        public DateTime DateCreated { get; set; }

        public int Odometer { get; set; }

        [ExcelDisplayName("Trip odometer")]
        public int TripOdometer { get; set; }

        [ExcelDisplayName("Quantity")]
        public double FuelQuantity { get; set; }

        [ExcelDisplayName("Fuel")]
        public string FuelTypeName { get; set; }

        public decimal Price { get; set; }

        [ExcelDisplayName("Currency")]
        public string CurrencyCode { get; set; }

        public double? Average { get; set; }
        
        public string Routes { get; set; }

        public string Note { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<FuelEntryDetailsModel, FuelEntryExcelViewModel>()
                .ForMember(dest => dest.Routes,
                    opt => opt.MapFrom(src => string.Join(", ", src.Routes)))
                .ForMember(dest => dest.FuelQuantity,
                    opt => opt.AddTransform(value => Math.Round(value, 2)))
                .ForMember(dest => dest.Price,
                    opt => opt.AddTransform(value => Math.Round(value, 2)))
                .ForMember(dest => dest.Average,
                    opt => opt.AddTransform(value => Math.Round(value ?? 0, 2)));
        }
    }
}
