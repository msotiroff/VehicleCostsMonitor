﻿namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Areas.Vehicle.Models;
    using AutoMapper;
    using Infrastructure.Collections;
    using Infrastructure.Filters;
    using Infrastructure.Providers.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Interfaces;
    using Services.Models.Entries.Interfaces;
    using Services.Models.Vehicle;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Models;
    using static VehicleCostsMonitor.Models.Common.ModelConstants;
    using static WebConstants;

    [Authorize]
    [Route("[area]/[action]/{id?}")]
    public class HomeController : BaseVehicleController
    {
        private readonly UserManager<User> userManager;
        private readonly IVehicleService vehicles;
        private readonly IManufacturerService manufacturers;
        private readonly IVehicleModelService models;
        private readonly IVehicleElementService vehicleElements;
        private readonly IDateTimeProvider dateTimeProvider;

        public HomeController(
            UserManager<User> userManager,
            IVehicleService vehicles,
            IManufacturerService manufacturers,
            IVehicleModelService models,
            IVehicleElementService vehicleElements,
            IDateTimeProvider dateTimeProvider)
        {
            this.userManager = userManager;
            this.vehicles = vehicles;
            this.manufacturers = manufacturers;
            this.models = models;
            this.vehicleElements = vehicleElements;
            this.dateTimeProvider = dateTimeProvider;
        }
        
        [HttpGet]
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

            var model = Mapper.Map<VehicleDetailsViewModel>(vehicle);
            var entriesQuery = this.vehicles.GetEntries(id);

            pageIndex = Math.Max(1, pageIndex);
            var totalPages = (int)(Math.Ceiling(entriesQuery.Count() / (double)EntriesListPageSize));
            pageIndex = Math.Min(pageIndex, Math.Max(1, totalPages));

            var entriesToShow = entriesQuery
                .Skip((pageIndex - 1) * EntriesListPageSize)
                .Take(EntriesListPageSize)
                .ToList();

            model.Entries = new PaginatedList<IEntryModel>(entriesToShow, pageIndex, totalPages);

            return View(model);
        }

        [HttpGet]
        [AuthorizeOwner]
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
        [AuthorizeOwner]
        [ValidateModelState]
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
        [AuthorizeOwner]
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
        [AuthorizeOwner]
        [ActionName(nameof(Delete))]
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

        [ResponseCache(Duration = 3600)]
        private async Task<VehicleCreateViewModel> InitializeCreationModel()
        {
            var allManufacturers = await this.manufacturers.AllAsync();
            var allVehicleTypes = await this.vehicleElements.GetVehicleTypes();
            var allFuelTypes = await this.vehicleElements.GetFuelTypes();
            var allGearingTypes = await this.vehicleElements.GetGearingTypes();
            var allAvailableYears = Enumerable
                .Range(YearOfManufactureMinValue, this.dateTimeProvider.GetCurrentDateTime().Year - YearOfManufactureMinValue + 1)
                .Select(y => y.ToString());

            var model = new VehicleCreateViewModel();

            model.AllManufacturers = allManufacturers.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            model.AllVehicleTypes = allVehicleTypes.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            model.AllFuelTypes = allFuelTypes.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            model.AllGearingTypes = allGearingTypes.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            model.AvailableYears = allAvailableYears.Select(y => new SelectListItem(y, y));

            return model;
        }

        [ResponseCache(Duration = 3600)]
        private async Task<VehicleUpdateViewModel> InitializeEditionModel(VehicleUpdateViewModel model)
        {
            var allManufacturers = await this.manufacturers.AllAsync();
            var allVehicleTypes = await this.vehicleElements.GetVehicleTypes();
            var allFuelTypes = await this.vehicleElements.GetFuelTypes();
            var allGearingTypes = await this.vehicleElements.GetGearingTypes();
            var allAvailableYears = Enumerable
                .Range(YearOfManufactureMinValue, this.dateTimeProvider.GetCurrentDateTime().Year - YearOfManufactureMinValue + 1)
                .Select(y => y.ToString());
            
            model.AllManufacturers = allManufacturers.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            model.AllVehicleTypes = allVehicleTypes.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            model.AllFuelTypes = allFuelTypes.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            model.AllGearingTypes = allGearingTypes.Select(m => new SelectListItem(m.Name, m.Id.ToString()));
            model.AvailableYears = allAvailableYears.Select(y => new SelectListItem(y, y));

            return model;
        }
    }
}