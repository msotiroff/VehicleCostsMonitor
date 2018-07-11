namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Manufacturer;
    using VehicleCostsMonitor.Web.Infrastructure.Filters;

    public class ManufacturerController : BaseAdminController
    {
        private readonly IManufacturerService manufacturers;

        public ManufacturerController(IManufacturerService manufacturers)
        {
            this.manufacturers = manufacturers;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await this.manufacturers.AllAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.manufacturers.GetDetailedAsync(id);
            if (model == null)
            {
                this.ShowNotification(string.Format(NotificationMessages.ManufacturerDoesNotExist, id));

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var success = await this.manufacturers.CreateAsync(name);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(string.Format(
                    NotificationMessages.ManufacturerCreatedSuccessfull, name),
                    NotificationType.Success);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await this.manufacturers.GetAsync(id);
            if (model == null)
            {
                this.ShowNotification(string.Format(NotificationMessages.ManufacturerDoesNotExist, id));

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Edit(ManufacturerUpdateServiceModel model)
        {
            var success = await this.manufacturers.UpdateAsync(model.Id, model.Name);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(string.Format(
                    NotificationMessages.ManufacturerUpdatedSuccessfull, model.Name),
                    NotificationType.Success);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.manufacturers.GetAsync(id);
            if (model == null)
            {
                this.ShowNotification(string.Format(NotificationMessages.ManufacturerDoesNotExist, id));

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Delete(ManufacturerUpdateServiceModel model)
        {
            var success = await this.manufacturers.DeleteAsync(model.Id);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(string.Format(
                    NotificationMessages.ManufacturerDeletedSuccessfull, model.Name),
                    NotificationType.Success);
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}