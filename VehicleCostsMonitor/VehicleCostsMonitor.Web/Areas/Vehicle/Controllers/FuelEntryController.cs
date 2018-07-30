namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Entries.FuelEntries;
    using VehicleCostsMonitor.Web.Areas.Vehicle.Models.Enums;
    using VehicleCostsMonitor.Web.Areas.Vehicle.Models.FuelEntry;
    using VehicleCostsMonitor.Web.Infrastructure.Filters;
    using VehicleCostsMonitor.Web.Infrastructure.Providers.Interfaces;

    [Authorize]
    // TODO: Implement [Authorize(Roles = "Owner")]
    public class FuelEntryController : BaseVehicleController
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IFuelEntryService fuelEntries;

        public FuelEntryController(IDateTimeProvider dateTimeProvider, IFuelEntryService fuelEntries)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.fuelEntries = fuelEntries;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var allFuelEntryTypes = await this.fuelEntries.GetEntryTypes();
            var allFuelTypes = await this.fuelEntries.GetFuelTypes();
            var allRouteTypes = await this.fuelEntries.GetRouteTypes();
            var allExtraFuelConsumers = await this.fuelEntries.GetExtraFuelConsumers();

            var model = new FuelEntryCreateViewModel
            {
                DateCreated = this.dateTimeProvider.GetCurrentDateTime(),
                VehicleId = id,
                FuelEntryTypes = allFuelEntryTypes.Select(fet => new SelectListItem(fet.Name, fet.Id.ToString())),
                FuelTypes = allFuelTypes.Select(ft => new SelectListItem(ft.Name, ft.Id.ToString())),
                AllRoutes = allRouteTypes,
                AllExraFuelConsumers = allExtraFuelConsumers,
                PricingTypes = Enum.GetNames(typeof(PricingType)).Select(pt => new SelectListItem(pt.ToString(), pt.ToString())),
            };

            model.LastOdometer = await this.fuelEntries.GetPreviousOdometerValue(model.VehicleId, model.DateCreated);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FuelEntryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.ShowModelStateError();
                return RedirectToVehicle(model.VehicleId);
            }
            
            var serviceModel = Mapper.Map<FuelEntryCreateServiceModel>(model);
            bool success = await this.fuelEntries.CreateAsync(serviceModel);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.FuelEntryUpdateFailed);
                return RedirectToAction(nameof(Create), new { id = model.VehicleId });
            }

            this.ShowNotification(NotificationMessages.FuelEntryAddedSuccessfull, NotificationType.Success);
            return RedirectToVehicle(model.VehicleId);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var fuelEntry = await this.fuelEntries.GetAsync(id);
            if (fuelEntry == null)
            {
                this.ShowNotification(NotificationMessages.FuelEntryDoesNotExist);
                return RedirectToProfile();
            }

            var allFuelEntryTypes = await this.fuelEntries.GetEntryTypes();
            var allFuelTypes = await this.fuelEntries.GetFuelTypes();
            var allRouteTypes = await this.fuelEntries.GetRouteTypes();
            var allExtraFuelConsumers = await this.fuelEntries.GetExtraFuelConsumers();
            var lastOdometer = await this.fuelEntries.GetPreviousOdometerValue(fuelEntry.VehicleId, fuelEntry.DateCreated);

            var model = Mapper.Map<FuelEntryUpdateViewModel>(fuelEntry);
            model.FuelEntryTypes = allFuelEntryTypes.Select(fet => new SelectListItem(fet.Name, fet.Id.ToString()));
            model.FuelTypes = allFuelTypes.Select(ft => new SelectListItem(ft.Name, ft.Id.ToString()));
            model.AllRoutes = allRouteTypes;
            model.AllExraFuelConsumers = allExtraFuelConsumers;
            model.LastOdometer = lastOdometer;
            model.PricingTypes = Enum.GetNames(typeof(PricingType)).Select(pt => new SelectListItem(pt.ToString(), pt.ToString()));

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Edit(FuelEntryUpdateViewModel model)
        {
            var fuelEntry = Mapper.Map<FuelEntry>(model);
            var success = await this.fuelEntries.UpdateAsync(fuelEntry);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.FuelEntryUpdateFailed);
            }

            this.ShowNotification(NotificationMessages.FuelEntryUpdatedSuccessfull, NotificationType.Success);

            return RedirectToVehicle(model.VehicleId);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.fuelEntries.GetForDeleteAsync(id);
            if (model == null)
            {
                this.ShowNotification(NotificationMessages.FuelEntryDoesNotExist);
                return RedirectToProfile();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Delete(FuelEntryDeleteServiceModel model)
        {
            bool success = await this.fuelEntries.DeleteAsync(model.Id);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.FuelEntryDeleteFailed);
            }

            this.ShowNotification(NotificationMessages.FuelEntryDeletedSuccessfull, NotificationType.Success);

            return RedirectToVehicle(model.VehicleId);
        }
    }
}
