namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Areas.Vehicle.Models.CostEntry;
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
    using Services.Models.Entries.CostEntries;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Web.Infrastructure.Extensions;

    [Authorize]
    public class CostEntryController : BaseEntryController
    {
        private const string CostEntryTypesCacheKey = "_CostEntryTypesStoredInCache";

        private readonly UserManager<User> userManager;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ICostEntryService costEntryService;

        public CostEntryController(
            IDistributedCache cache,
            UserManager<User> userManager,
            IDateTimeProvider dateTimeProvider, 
            ICostEntryService costEntries,
            ICurrencyService currencyService)
            : base(cache, currencyService)
        {
            this.userManager = userManager;
            this.dateTimeProvider = dateTimeProvider;
            this.costEntryService = costEntries;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var user = await this.userManager.GetUserAsync(User);

            var model = new CostEntryCreateViewModel
            {
                DateCreated = this.dateTimeProvider.GetCurrentDateTime(),
                VehicleId = id,
                CurrencyId = user.CurrencyId,
                AllCostEntryTypes = await this.GetAllCostEntryTypes(),
                AllCurrencies = await this.GetAllCurrenciesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Create(CostEntryCreateViewModel model)
        {
            var success = await this.costEntryService
                .CreateAsync(model.DateCreated, model.CostEntryTypeId, model.VehicleId, model.Price, model.CurrencyId.Value, model.Note, model.Odometer);

            if (!success)
            {
                this.ShowNotification(NotificationMessages.CostEntryUpdateFailed);
                return RedirectToAction(nameof(Create), new { id = model.VehicleId });
            }

            this.ShowNotification(NotificationMessages.CostEntryAddedSuccessfull, NotificationType.Success);

            return RedirectToVehicle(model.VehicleId);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var costEntry = await this.costEntryService.GetForUpdateAsync(id);
            if (costEntry == null)
            {
                this.ShowNotification(NotificationMessages.CostEntryDoesNotExist);
                this.RedirectToProfile();
            }

            var model = Mapper.Map<CostEntryUpdateViewModel>(costEntry);
            model.AllCostEntryTypes = await this.GetAllCostEntryTypes();
            model.AllCurrencies = await this.GetAllCurrenciesAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Edit(CostEntryUpdateViewModel model)
        {
            var success = await this.costEntryService
                .UpdateAsync(model.Id, model.DateCreated, model.CostEntryTypeId, model.Price, model.CurrencyId, model.Note, model.Odometer);

            if (!success)
            {
                this.ShowNotification(NotificationMessages.CostEntryUpdateFailed);
            }
            else
            {
                this.ShowNotification(NotificationMessages.CostEntryUpdatedSuccessfull, NotificationType.Success);
            }

            return RedirectToVehicle(model.VehicleId);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.costEntryService.GetForDeleteAsync(id);

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Delete(CostEntryDeleteServiceModel model)
        {
            bool success = await this.costEntryService.DeleteAsync(model.Id);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.CostEntryDeleteFailed);
            }
            else
            {
                this.ShowNotification(NotificationMessages.CostEntryDeletedSuccessfull, NotificationType.Success);
            }

            return RedirectToVehicle(model.VehicleId);
        }

        private async Task<IEnumerable<SelectListItem>> GetAllCostEntryTypes()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.Cache.GetStringAsync(CostEntryTypesCacheKey);
            if (listFromCache == null)
            {
                var costEntries = await this.costEntryService.GetEntryTypesAsync();
                list = costEntries.Select(x => new SelectListItem(x.Name.ToString(), x.Id.ToString()));
                var expiration = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays);

                await this.Cache.SetSerializableObject(CostEntryTypesCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }
    }
}