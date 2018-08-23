namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models.CostEntry
{
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
    using Infrastructure.Extensions.ExcelExport.Attributes;
    using Services.Models.Entries;
    using System;

    public class CostEntryExcelViewModel : IAutoMapWith<CostEntryDetailsModel>, ICustomMappingConfiguration
    {
        [ExcelDisplayName("Date")]
        [ExcelValueFormat("dd-MM-yyyy")]
        public DateTime DateCreated { get; set; }

        public int? Odometer { get; set; }

        [ExcelDisplayName("Type")]
        public string CostEntryTypeName { get; set; }

        public decimal Price { get; set; }

        [ExcelDisplayName("Currency")]
        public string CurrencyCode { get; set; }

        public string Note { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<CostEntryDetailsModel, CostEntryExcelViewModel>()
                .ForMember(dest => dest.Price,
                    opt => opt.AddTransform(value => Math.Round(value, 2)));
        }
    }
}
