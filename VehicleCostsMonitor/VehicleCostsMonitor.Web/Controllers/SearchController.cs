namespace VehicleCostsMonitor.Web.Controllers
{
    using Infrastructure.Collections;
    using Infrastructure.Utilities.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Services.Interfaces;
    using Services.Models.Vehicle;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Web.Models.Search;
    using static VehicleCostsMonitor.Models.Common.ModelConstants;
    using static WebConstants;

    public class SearchController : BaseController
    {
        private readonly IVehicleService vehicles;
        private readonly IManufacturerService manufacturers;
        private readonly IVehicleElementService vehicleElements;
        private readonly IDateTimeProvider dateTimeProvider;

        public SearchController(
            IVehicleService vehicles, 
            IManufacturerService manufacturers, 
            IVehicleElementService vehicleElements,
            IDateTimeProvider dateTimeProvider)
        {
            this.vehicles = vehicles;
            this.manufacturers = manufacturers;
            this.vehicleElements = vehicleElements;
            this.dateTimeProvider = dateTimeProvider;
        }

        public IActionResult Index() 
            => RedirectToAction(nameof(this.Result), new { manufacturerId = default(int), modelName = string.Empty });

        public async Task<IActionResult> Result(
            int manufacturerId, 
            string modelName, 
            string exactModelName,
            int vehicleTypeId,
            int fuelTypeId,
            int gearingTypeId,
            int engineHorsePowerMin,
            int engineHorsePowerMax,
            int yearOfManufactureMin,
            int yearOfManufactureMax,
            int minimumKilometers,
            int pageIndex = 1)
        {
            var vehicles = this.vehicles
                .Get(manufacturerId, modelName, exactModelName, vehicleTypeId, fuelTypeId, gearingTypeId, engineHorsePowerMin, engineHorsePowerMax, yearOfManufactureMin, yearOfManufactureMax, minimumKilometers);

            pageIndex = Math.Max(1, pageIndex);
            var totalPages = (int)(Math.Ceiling(vehicles.Count() / (double)SearchResultsPageSize));
            pageIndex = Math.Min(pageIndex, Math.Max(1, totalPages));

            var model = await InitializeSearchModel(
                manufacturerId, 
                modelName, 
                exactModelName, 
                engineHorsePowerMin, 
                engineHorsePowerMax, 
                yearOfManufactureMin, 
                yearOfManufactureMax, 
                minimumKilometers, 
                pageIndex, 
                vehicles, 
                totalPages);

            return View(model);
        }

        private async Task<SearchViewModel> InitializeSearchModel(
            int manufacturerId, 
            string modelName, 
            string exactModelName, 
            int engineHorsePowerMin, 
            int engineHorsePowerMax, 
            int yearOfManufactureMin, 
            int yearOfManufactureMax, 
            int minimumKilometers, 
            int pageIndex, 
            IQueryable<VehicleSearchServiceModel> vehicles, 
            int totalPages)
        {
            var vehiclesToShow = vehicles
                            .Skip((pageIndex - 1) * SearchResultsPageSize)
                            .Take(SearchResultsPageSize)
                            .ToList();

            var results = new PaginatedList<VehicleSearchServiceModel>(vehiclesToShow, pageIndex, totalPages);

            var allManufacturers = await this.manufacturers.AllAsync();
            var availableYears = Enumerable.Range(YearOfManufactureMinValue, this.dateTimeProvider.GetCurrentDateTime().Year - YearOfManufactureMinValue + 1);
            var allVehicleTypes = await this.vehicleElements.GetVehicleTypes();
            var allFuelTypes = await this.vehicleElements.GetFuelTypes();
            var allGearingTypes = await this.vehicleElements.GetGearingTypes();

            var model = new SearchViewModel(
                manufacturerId, 
                modelName, 
                exactModelName, 
                allManufacturers, 
                availableYears, 
                yearOfManufactureMin, 
                yearOfManufactureMax, 
                allVehicleTypes, 
                allFuelTypes, 
                allGearingTypes, 
                engineHorsePowerMin, 
                engineHorsePowerMax, 
                minimumKilometers, 
                results);

            return model;
        }
    }
}