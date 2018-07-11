﻿namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Services.Interfaces;
    using Services.Models.VehicleModel;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;

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
            var success = await this.vehicleModels.CreateAsync(name, manufacturerId);
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
            var model = await this.vehicleModels.GetAsync(id);
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
            var success = await this.vehicleModels.DeleteAsync(model.Id);
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