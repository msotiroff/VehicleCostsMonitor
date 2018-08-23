namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Areas.Vehicle.Models.FuelEntry;
    using AutoMapper;
    using Common.Notifications;
    using Infrastructure.Filters;
    using Infrastructure.Utilities.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models.Entries.FuelEntries;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Web.Infrastructure.Extensions;
    using static WebConstants;

    [Authorize]
    public class FuelEntryController : BaseEntryController
    {
        private const string FuelEntryTypesCacheKey = "_FuelEntryTypesStoredInCache";
        private const string RouteTypesCacheKey = "_RouteTypesStoredInCache";
        private const string ExtraFuelConsumersCacheKey = "_ExtraFuelConsumersStoredInCache";

        private readonly UserManager<User> userManager;
        private readonly IFuelEntryService fuelEntryService;
        private readonly IDateTimeProvider dateTimeProvider;

        public FuelEntryController(
            IDistributedCache cache, 
            IFuelEntryService fuelEntryService, 
            ICurrencyService currencies,
            UserManager<User> userManager,
            IDateTimeProvider dateTimeProvider) 
            : base(cache, currencies)
        {
            this.userManager = userManager;
            this.fuelEntryService = fuelEntryService;
            this.dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var user = await this.userManager.GetUserAsync(User);
            DateTime dateCreated = this.dateTimeProvider.GetCurrentDateTime();

            var model = new FuelEntryCreateViewModel
            {
                DateCreated = dateCreated,
                VehicleId = id,
                CurrencyId = user.CurrencyId,
                FuelEntryTypes = await this.GetAllFuelEntryTypesAsync(),
                FuelTypes = await this.GetAllFuelTypesAsync(),
                AllRoutes = await this.GetAllRouteTypesAsync(),
                AllExraFuelConsumers = await this.GetAllExtraFuelConsumersAsync(),
                AllCurrencies = await this.GetAllCurrenciesAsync(),
                PricingTypes = await this.GetAllPricingTypesAsync(),
                LastOdometer = await this.GetPreviousOdometerValue(id, dateCreated)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FuelEntryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.ShowModelStateError();
                return RedirectToAction(nameof(Create), new { id = model.VehicleId });
            }

            var serviceModel = Mapper.Map<FuelEntryCreateServiceModel>(model);
            var success = await this.fuelEntryService.CreateAsync(serviceModel);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.FuelEntryUpdateFailed);
                return RedirectToAction(nameof(Create), new { id = model.VehicleId });
            }

            this.ShowNotification(NotificationMessages.FuelEntryAddedSuccessfull, NotificationType.Success);
            return RedirectToVehicle(model.VehicleId);
        }

        [HttpGet]
        [EnsureOwnership]
        public async Task<IActionResult> Edit(int id)
        {
            var fuelEntry = await this.fuelEntryService.GetAsync(id);
            if (fuelEntry == null)
            {
                this.ShowNotification(NotificationMessages.FuelEntryDoesNotExist);
                return RedirectToProfile();
            }

            var model = Mapper.Map<FuelEntryUpdateViewModel>(fuelEntry);

            model.FuelEntryTypes = await this.GetAllFuelEntryTypesAsync();
            model.FuelTypes = await this.GetAllFuelTypesAsync();
            model.AllRoutes = await this.GetAllRouteTypesAsync();
            model.AllExraFuelConsumers = await this.GetAllExtraFuelConsumersAsync();
            model.AllCurrencies = await this.GetAllCurrenciesAsync();
            model.LastOdometer = await this.GetPreviousOdometerValue(fuelEntry.VehicleId, fuelEntry.DateCreated);
            model.PricingTypes = await this.GetAllPricingTypesAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        [EnsureOwnership]
        public async Task<IActionResult> Edit(FuelEntryUpdateViewModel model)
        {
            var fuelEntry = Mapper.Map<FuelEntry>(model);
            var success = await this.fuelEntryService.UpdateAsync(fuelEntry);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.FuelEntryUpdateFailed);
            }

            this.ShowNotification(NotificationMessages.FuelEntryUpdatedSuccessfull, NotificationType.Success);

            return RedirectToVehicle(model.VehicleId);
        }

        [HttpGet]
        [EnsureOwnership]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.fuelEntryService.GetForDeleteAsync(id);
            if (model == null)
            {
                this.ShowNotification(NotificationMessages.FuelEntryDoesNotExist);
                return RedirectToProfile();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        [EnsureOwnership]
        public async Task<IActionResult> Delete(FuelEntryDeleteServiceModel model)
        {
            bool success = await this.fuelEntryService.DeleteAsync(model.Id);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.FuelEntryDeleteFailed);
            }

            this.ShowNotification(NotificationMessages.FuelEntryDeletedSuccessfull, NotificationType.Success);

            return RedirectToVehicle(model.VehicleId);
        }

        #region Private methods
        private async Task<int> GetPreviousOdometerValue(int vehicleId, DateTime dateCreated)
        {
            return await this.fuelEntryService.GetPreviousOdometerValue(vehicleId, dateCreated);
        }

        private async Task<IEnumerable<SelectListItem>> GetAllFuelEntryTypesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.Cache.GetStringAsync(FuelEntryTypesCacheKey);
            if (listFromCache == null)
            {
                var entryTypes = await this.fuelEntryService.GetEntryTypes();
                list = entryTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
                var expiration = TimeSpan.FromDays(StaticElementsCacheExpirationInDays);

                await this.Cache.SetSerializableObject(FuelEntryTypesCacheKey, list, expiration);
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

            var listFromCache = await this.Cache.GetStringAsync(FuelTypesCacheKey);
            if (listFromCache == null)
            {
                var fuelTypes = await this.fuelEntryService.GetFuelTypes();
                list = fuelTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
                var expiration = TimeSpan.FromDays(StaticElementsCacheExpirationInDays);
                
                await this.Cache.SetSerializableObject(FuelTypesCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        private async Task<IEnumerable<RouteType>> GetAllRouteTypesAsync()
        {
            IEnumerable<RouteType> list;

            var listFromCache = await this.Cache.GetStringAsync(RouteTypesCacheKey);
            if (listFromCache == null)
            {
                list = await this.fuelEntryService.GetRouteTypes();
                var expiration = TimeSpan.FromDays(StaticElementsCacheExpirationInDays);
                
                await this.Cache.SetSerializableObject(RouteTypesCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<RouteType>>(listFromCache);
            }

            return list;
        }

        private async Task<IEnumerable<ExtraFuelConsumer>> GetAllExtraFuelConsumersAsync()
        {
            IEnumerable<ExtraFuelConsumer> list;

            var listFromCache = await this.Cache.GetStringAsync(ExtraFuelConsumersCacheKey);
            if (listFromCache == null)
            {
                list = await this.fuelEntryService.GetExtraFuelConsumers();
                var expiration = TimeSpan.FromDays(StaticElementsCacheExpirationInDays);
                
                await this.Cache.SetSerializableObject(ExtraFuelConsumersCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<ExtraFuelConsumer>>(listFromCache);
            }

            return list;
        }
        #endregion
    }
}
