namespace VehicleCostsMonitor.Web.Models.Search
{
    using Infrastructure.Attributes;
    using Infrastructure.Collections;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.Manufacturer;
    using Services.Models.Vehicle;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using VehicleCostsMonitor.Models;
    using static VehicleCostsMonitor.Models.Common.ModelConstants;

    public class SearchViewModel
    {
        private const int DefaultMinimumKilometers = 1500;

        public SearchViewModel(
            int manufacturerId,
            string modelName,
            string exactModelName,
            IEnumerable<ManufacturerConciseListModel> allManufacturers,
            IEnumerable<int> availableYears,
            int yearMin,
            int yearMax,
            IEnumerable<VehicleType> allVehicleTypes,
            IEnumerable<FuelType> allFuelTypes,
            IEnumerable<GearingType> allGearingTypes,
            int powerMin,
            int powerMax,
            int minimumKilometers,
            PaginatedList<VehicleSearchServiceModel> results)
        {
            this.ManufacturerId = manufacturerId;
            this.ModelName = modelName;
            this.AllManufacturers = allManufacturers.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            this.AvailableYears = availableYears.Select(y => new SelectListItem(y.ToString(), y.ToString()));
            this.ExactModelname = exactModelName ?? string.Empty;
            this.AllVehicleTypes = allVehicleTypes.Select(vt => new SelectListItem(vt.Name, vt.Id.ToString()));
            this.AllFuelTypes = allFuelTypes.Select(ft => new SelectListItem(ft.Name, ft.Id.ToString()));
            this.AllGearingTypes = allGearingTypes.Select(gt => new SelectListItem(gt.Name, gt.Id.ToString()));
            this.MinimumKilometers = minimumKilometers != default(int) ? minimumKilometers : DefaultMinimumKilometers;
            this.EngineHorsePowerMin = powerMin != default(int) ? powerMin : EngineHorsePowerMinValue;
            this.EngineHorsePowerMax = powerMax != default(int) ? powerMax : EngineHorsePowerMaxValue;
            this.YearOfManufactureMin = yearMin != default(int) ? yearMin : availableYears.Min();
            this.YearOfManufactureMax = yearMax != default(int) ? yearMax : availableYears.Max();
            this.Results = results;
        }

        [Display(Name = "Make")]
        public int ManufacturerId { get; set; }

        public IEnumerable<SelectListItem> AllManufacturers { get; set; }
        
        [Display(Name = "Model")]
        public string ModelName { get; set; }

        [Display(Name = "Exact model name")]
        public string ExactModelname { get; set; }
        
        [Display(Name = "Year from")]
        public int YearOfManufactureMin { get; set; }

        [Display(Name = "Year to")]
        [EqualOrGreaterThan(nameof(YearOfManufactureMin))]
        public int YearOfManufactureMax { get; set; }

        public IEnumerable<SelectListItem> AvailableYears { get; set; }
        
        [Display(Name = "Power from")]
        public int EngineHorsePowerMin { get; set; }

        [Display(Name = "Power to")]
        public int EngineHorsePowerMax { get; set; }

        [Display(Name = "Vehicle type")]
        public int VehicleTypeId { get; set; }

        public IEnumerable<SelectListItem> AllVehicleTypes { get; set; }

        [Required]
        [Display(Name = "Fuel type")]
        public int FuelTypeId { get; set; }

        public IEnumerable<SelectListItem> AllFuelTypes { get; set; }

        [Required]
        [Display(Name = "Gearing type")]
        public int GearingTypeId { get; set; }

        public IEnumerable<SelectListItem> AllGearingTypes { get; set; }

        [Display(Name = "Minimum kilometers")]
        public int MinimumKilometers { get; set; }

        public PaginatedList<VehicleSearchServiceModel> Results { get; set; }
    }
}
