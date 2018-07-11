namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.VehicleModel;
    using VehicleCostsMonitor.Web.Infrastructure.Filters;

    public class VehicleModelController : BaseAdminController
    {
        private readonly IVehicleModelService vehicleModels;

        public VehicleModelController(IVehicleModelService vehicleModels)
        {
            this.vehicleModels = vehicleModels;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, int manufacturerId)
        {
            var success = await this.vehicleModels.Create(name, manufacturerId);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(
                    string.Format(NotificationMessages.ModelCreatedSuccessfull, name),
                    NotificationType.Success);
            }

            return RedirectToAction(nameof(ManufacturerController.Index), "Manufacturer");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.vehicleModels.Get(id);
            if (model == null)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
                return RedirectToAction(nameof(ManufacturerController.Index), "Manufacturer");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Delete(ModelConciseServiceModel model)
        {
            var success = await this.vehicleModels.Delete(model.Id);
            if (success)
            {
                this.ShowNotification(
                    string.Format(NotificationMessages.ModelDeletedSuccessfull, model.Name),
                    NotificationType.Success);
            }
            else
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }

            return RedirectToAction(nameof(ManufacturerController.Index), "Manufacturer");
        }
    }
}