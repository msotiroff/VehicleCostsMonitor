namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models.Evaluation
{
    using AutoMapper;
    using Common.AutoMapping.Interfaces;
    using System;
    using VehicleCostsMonitor.Services.Models.Vehicle;
    using VehicleCostsMonitor.Web.Infrastructure.Extensions.ExcelExport.Attributes;

    public class VehicleStatisticExcelViewModel : IAutoMapWith<VehicleStatisticServiceModel>, ICustomMappingConfiguration
    {
        [ExcelIgnore]
        public int ManufacturerId { get; set; }

        [ExcelDisplayName("Manufacturer")]
        public string ManufacturerName { get; set; }

        [ExcelDisplayName("Model")]
        public string ModelName { get; set; }

        [ExcelDisplayName("Fuel")]
        public string FuelType { get; set; }
        
        public int Count { get; set; }

        public double Average { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<VehicleStatisticServiceModel, VehicleStatisticExcelViewModel>()
                .ForMember(dest => dest.Average,
                    opt => opt.AddTransform(value => Math.Round(value, 2)));
        }
    }
}
