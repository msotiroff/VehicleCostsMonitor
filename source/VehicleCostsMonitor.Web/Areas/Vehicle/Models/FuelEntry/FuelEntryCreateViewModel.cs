namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models.FuelEntry
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using VehicleCostsMonitor.Common.AutoMapping.Interfaces;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Entries.FuelEntries;
    using VehicleCostsMonitor.Web.Infrastructure.Attributes;

    public class FuelEntryCreateViewModel : IAutoMapWith<FuelEntryCreateServiceModel>, ICustomMappingConfiguration
    {
        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        [Required]
        [EqualOrGreaterThan(nameof(LastOdometer))]
        public int Odometer { get; set; }

        [Display(Name = "Previous odometer value")]
        public int LastOdometer { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public double FuelQuantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int? CurrencyId { get; set; }

        public IEnumerable<SelectListItem> AllCurrencies { get; set; }

        public string PricingType { get; set; }

        public IEnumerable<SelectListItem> PricingTypes { get; set; }

        public string Note { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int FuelEntryTypeId { get; set; }

        public IEnumerable<SelectListItem> FuelEntryTypes { get; set; }

        [Required]
        [Display(Name = "Fuel type")]
        public int FuelTypeId { get; set; }
        
        public IEnumerable<SelectListItem> FuelTypes { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Display(Name = "Route(s)")]
        public IEnumerable<int> RoutesIds { get; set; }
        
        public IEnumerable<RouteType> AllRoutes { get; set; }
        
        [Display(Name = "Extras")]
        public IEnumerable<int> ExtraFuelConsumersIds { get; set; }
        
        public IEnumerable<ExtraFuelConsumer> AllExraFuelConsumers { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper
                .CreateMap<FuelEntryCreateViewModel, FuelEntryCreateServiceModel>()
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.PricingType == Enums.PricingType.Total.ToString() 
                        ? src.Price 
                        : (src.Price * (decimal)src.FuelQuantity)))
                .ForMember(dest => dest.Routes, 
                    opt => opt.MapFrom(src => src.RoutesIds
                        .Select(rId => new FuelEntryRouteType { RouteTypeId = rId})))
                .ForMember(dest => dest.ExtraFuelConsumers, 
                    opt => opt.MapFrom(src => src.ExtraFuelConsumersIds
                        .Select(extraId => new FuelEntryExtraFuelConsumer { ExtraFuelConsumerId = extraId })));
        }
    }
}
