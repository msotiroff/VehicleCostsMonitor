namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Vehicle;
    using VehicleCostsMonitor.Web.Areas.Vehicle.Models;
    using VehicleCostsMonitor.Web.Infrastructure.Filters;
    using VehicleCostsMonitor.Web.Infrastructure.Providers.Interfaces;
    using static VehicleCostsMonitor.Models.Common.ModelConstants;

    [Authorize]
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

        public IActionResult Details(int id)
        {
            throw new NotImplementedException();
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
    }
}