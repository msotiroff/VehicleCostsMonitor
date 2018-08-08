namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Areas.Vehicle.Models;
    using AutoMapper;
    using Infrastructure.Collections;
    using Infrastructure.Filters;
    using Infrastructure.Utilities.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models.Entries.Interfaces;
    using Services.Models.Vehicle;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Web.Infrastructure.Extensions;
    using static VehicleCostsMonitor.Models.Common.ModelConstants;
    using static WebConstants;

    [Authorize]
    [Route("[area]/[action]/{id?}")]
    public class VehicleController : BaseVehicleController
    {
        private const string GearingTypesCacheKey = "_GearingTypesStoredInCache";
        private const string VehicleTypesCacheKey = "_VehicleTypesStoredInCache";

        private readonly UserManager<User> userManager;
        private readonly IDistributedCache cache;
        private readonly IVehicleService vehicles;
        private readonly IManufacturerService manufacturers;
        private readonly IVehicleModelService models;
        private readonly IVehicleElementService vehicleElements;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ICurrencyExchanger currencyExchanger;

        public VehicleController(
            UserManager<User> userManager,
            IDistributedCache cache,
            IVehicleService vehicles,
            IManufacturerService manufacturers,
            IVehicleModelService models,
            IVehicleElementService vehicleElements,
            IDateTimeProvider dateTimeProvider,
            ICurrencyExchanger currencyExchanger)
        {
            this.userManager = userManager;
            this.cache = cache;
            this.vehicles = vehicles;
            this.manufacturers = manufacturers;
            this.models = models;
            this.vehicleElements = vehicleElements;
            this.dateTimeProvider = dateTimeProvider;
            this.currencyExchanger = currencyExchanger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetModelsByManufacturerId(int manufacturerId)
        {
            var models = await this.models.GetByManufacturerIdAsync(manufacturerId);

            return Json(new SelectList(models));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await this.InitializeCreationModel();

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Create(VehicleCreateViewModel model)
        {
            model.UserId = this.userManager.GetUserId(User);

            var serviceModel = Mapper.Map<VehicleCreateServiceModel>(model);

            var newVehicleId = await this.vehicles.CreateAsync(serviceModel);

            return RedirectToAction(nameof(Details), new { id = newVehicleId });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id, int pageIndex = 1)
        {
            var vehicle = await this.vehicles.GetAsync(id);
            if (vehicle == null)
            {
                this.ShowNotification(NotificationMessages.VehicleDoesNotExist);
                return RedirectToHome();
            }

            var model = this.InitializeDetailedModel(vehicle, pageIndex);

            return View(model);
        }

        [HttpGet]
        [EnsureOwnership]
        public async Task<IActionResult> Edit(int id)
        {
            var vehicle = await this.vehicles.GetForUpdateAsync(id);
            if (vehicle == null)
            {
                this.ShowNotification(NotificationMessages.VehicleDoesNotExist);
                this.RedirectToHome();
            }

            var model = await this.InitializeEditionModel(Mapper.Map<VehicleUpdateViewModel>(vehicle));

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        [EnsureOwnership]
        public async Task<IActionResult> Edit(VehicleUpdateViewModel model)
        {
            var serviceModel = Mapper.Map<VehicleUpdateServiceModel>(model);

            var success = await this.vehicles.UpdateAsync(serviceModel);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }

            this.ShowNotification(NotificationMessages.VehicleUpdatedSuccessfull, NotificationType.Success);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpGet]
        [EnsureOwnership]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.vehicles.GetAsync(id);
            if (model == null)
            {
                this.ShowNotification(NotificationMessages.VehicleDoesNotExist);
                this.RedirectToHome();
            }

            return View(model);
        }

        [HttpPost]
        [ActionName(nameof(Delete))]
        [EnsureOwnership]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await this.vehicles.DeleteAsync(id);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
                return RedirectToAction(nameof(Delete), new { id });
            }

            this.ShowNotification(NotificationMessages.VehicleDeletedSuccessfull, NotificationType.Success);
            return RedirectToAction("index", "profile", new { area = "user", id = this.userManager.GetUserId(User) });
        }

        #region Private methods
        private async Task<VehicleCreateViewModel> InitializeCreationModel()
        {
            var allManufacturers = await this.manufacturers.AllAsync();

            var model = new VehicleCreateViewModel
            {
                AllManufacturers = allManufacturers.Select(m => new SelectListItem(m.Name, m.Id.ToString())),
                AllVehicleTypes = await this.GetAllVehicleTypesAsync(),
                AllFuelTypes = await this.GetAllFuelTypesAsync(),
                AllGearingTypes = await this.GetAllGearingTypesAsync(),
                AvailableYears = this.GetAvailableYears()
            };

            return model;
        }

        private async Task<VehicleUpdateViewModel> InitializeEditionModel(VehicleUpdateViewModel model)
        {
            var allManufacturers = await this.manufacturers.AllAsync();

            model.AllManufacturers = allManufacturers.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            model.AllVehicleTypes = await this.GetAllVehicleTypesAsync();
            model.AllFuelTypes = await this.GetAllFuelTypesAsync();
            model.AllGearingTypes = await this.GetAllGearingTypesAsync();
            model.AvailableYears = this.GetAvailableYears();

            return model;
        }

        private VehicleDetailsViewModel InitializeDetailedModel(VehicleDetailsServiceModel vehicle, int pageIndex)
        {
            var allEntries = vehicle
                .FuelEntries
                .Cast<IEntryModel>()
                .Concat(vehicle.CostEntries
                    .Cast<IEntryModel>())
                .OrderByDescending(e => e.DateCreated)
                .ToList();

            pageIndex = Math.Max(1, pageIndex);
            var totalPages = (int)(Math.Ceiling(allEntries.Count() / (double)EntriesListPageSize));
            pageIndex = Math.Min(pageIndex, Math.Max(1, totalPages));

            var displayCurrency = vehicle.OwnerDisplayCurrency;

            var fuelEntriesToConvert = vehicle.FuelEntries
                .GroupBy(fe => fe.CurrencyCode)
                .ToDictionary(x => x.Key, x => x.Sum(fe => fe.Price));

            var fuelEntriesConverted = new Dictionary<string, decimal>();
            fuelEntriesToConvert
                .ForEach(kvp => fuelEntriesConverted[kvp.Key] = this.currencyExchanger.Convert(kvp.Key, kvp.Value, displayCurrency));
            var totalFuelCosts = fuelEntriesConverted.Sum(fe => fe.Value);

            var costEntriesToConvert = vehicle.CostEntries
                .GroupBy(fe => fe.CurrencyCode)
                .ToDictionary(x => x.Key, x => x.Sum(fe => fe.Price));

            var costEntriesConverted = new Dictionary<string, decimal>();
            costEntriesToConvert
                .ForEach(kvp => costEntriesConverted[kvp.Key] = this.currencyExchanger.Convert(kvp.Key, kvp.Value, displayCurrency));
            var totalOtherCosts = costEntriesConverted.Sum(kvp => kvp.Value);

            var costs = vehicle.CostEntries
                .ForEach(ce => ce.Price = this.currencyExchanger.Convert(ce.CurrencyCode, ce.Price, displayCurrency))
                .GroupBy(e => e.ToString())
                .ToDictionary(x => x.Key, y => y.Sum(e => e.Price));            
            costs.Add("Fuel", totalFuelCosts);

            var routes = vehicle.FuelEntries.SelectMany(fe => fe.Routes).GroupBy(r => r).ToDictionary(x => x.Key, x => x.Count());

            var minConsumption = vehicle.FuelEntries.Any(fe => fe.Average.Value > 0)
                ? vehicle.FuelEntries.Where(fe => fe.Average > 0).Min(fe => fe.Average.Value)
                : 0;

            var maxConsumption = vehicle.FuelEntries.Any(fe => fe.Average.Value > 0)
                ? vehicle.FuelEntries.Where(fe => fe.Average > 0).Max(fe => fe.Average.Value)
                : 0;

            var step = (maxConsumption - minConsumption) / ConsumptionHistogramRangesCount;

            var model = Mapper.Map<VehicleDetailsViewModel>(vehicle);
            model.Stats = new Statistics
            {
                Costs = costs,
                Routes = routes,
                MinConsumption = minConsumption,
                MaxConsumption = maxConsumption,
                ConsumptionRanges = new List<ConsumptionInRange>()
            };
            model.TotalFuelCosts = totalFuelCosts;
            model.TotalOtherCosts = totalOtherCosts;
            model.TotalSpent = totalFuelCosts + totalOtherCosts;
            model.TotalCostsPerHundredKm = model.TotalSpent / (model.TotalDistance != 0 ? model.TotalDistance : 100) * 100;

            for (int i = 0; i < ConsumptionHistogramRangesCount; i++)
            {
                model.Stats.ConsumptionRanges.Add(new ConsumptionInRange
                {
                    From = minConsumption,
                    To = minConsumption + step,
                    Count = vehicle.FuelEntries.Count(fe => fe.Average > 0 && fe.Average >= minConsumption && fe.Average <= minConsumption + step)
                });

                minConsumption += step;
            }

            model.Stats.MileageByDate = vehicle.FuelEntries
                .OrderByDescending(fe => fe.DateCreated)
                .Take(MileageByDateItemsCount)
                .Select(fe => new MileageByDate
                {
                    Date = fe.DateCreated,
                    Consumption = fe.Average ?? default(double)
                })
                .OrderBy(m => m.Date);


            var entriesToShow = allEntries
                .Skip((pageIndex - 1) * EntriesListPageSize)
                .Take(EntriesListPageSize)
                .ToList();

            model.Entries = new PaginatedList<IEntryModel>(entriesToShow, pageIndex, totalPages);
            return model;
        }

        private async Task<IEnumerable<SelectListItem>> GetAllGearingTypesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.cache.GetStringAsync(GearingTypesCacheKey);
            if (listFromCache == null)
            {
                var gearingTypes = await this.vehicleElements.GetGearingTypes();
                list = gearingTypes.Select(x => new SelectListItem(x.Name.ToString(), x.Id.ToString()));

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays)
                };

                await this.cache.SetStringAsync(GearingTypesCacheKey, JsonConvert.SerializeObject(list), options);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        private async Task<IEnumerable<SelectListItem>> GetAllFuelTypesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.cache.GetStringAsync(FuelTypesCacheKey);
            if (listFromCache == null)
            {
                var fuelTypes = await this.vehicleElements.GetFuelTypes();
                list = fuelTypes.Select(x => new SelectListItem(x.Name.ToString(), x.Id.ToString()));

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays)
                };

                await this.cache.SetStringAsync(FuelTypesCacheKey, JsonConvert.SerializeObject(list), options);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        private async Task<IEnumerable<SelectListItem>> GetAllVehicleTypesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.cache.GetStringAsync(VehicleTypesCacheKey);
            if (listFromCache == null)
            {
                var vehicleTypes = await this.vehicleElements.GetVehicleTypes();
                list = vehicleTypes.Select(x => new SelectListItem(x.Name.ToString(), x.Id.ToString()));

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays)
                };

                await this.cache.SetStringAsync(VehicleTypesCacheKey, JsonConvert.SerializeObject(list), options);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        private IEnumerable<SelectListItem> GetAvailableYears()
        {
            return Enumerable
                    .Range(YearOfManufactureMinValue, this.dateTimeProvider.GetCurrentDateTime().Year - YearOfManufactureMinValue + 1)
                    .Select(y => new SelectListItem(y.ToString(), y.ToString()));
        }
        #endregion
    }
}