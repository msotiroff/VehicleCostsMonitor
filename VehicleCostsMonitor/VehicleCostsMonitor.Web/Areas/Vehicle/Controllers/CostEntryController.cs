namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Entries.CostEntries;
    using VehicleCostsMonitor.Web.Areas.Vehicle.Models.CostEntry;
    using VehicleCostsMonitor.Web.Infrastructure.Filters;
    using VehicleCostsMonitor.Web.Infrastructure.Providers.Interfaces;

    [Authorize]
    // TODO: Implement [Authorize(Roles = "Owner")]
    public class CostEntryController : BaseVehicleController
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ICostEntryService costEntries;

        public CostEntryController(IDateTimeProvider dateTimeProvider, ICostEntryService costEntries)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.costEntries = costEntries;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var costEntryTypes = await this.costEntries.GetEntryTypesAsync();

            var model = new CostEntryCreateViewModel
            {
                DateCreated = this.dateTimeProvider.GetCurrentDateTime(),
                VehicleId = id,
                AllCostEntryTypes = await this.GetAllCostEntryTypes()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Create(CostEntryCreateViewModel model)
        {
            var success = await this.costEntries
                .CreateAsync(model.DateCreated, model.CostEntryTypeId, model.VehicleId, model.Price, model.Note, model.Odometer);

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
            var costEntry = await this.costEntries.GetForUpdateAsync(id);
            if (costEntry == null)
            {
                this.ShowNotification(NotificationMessages.CostEntryDoesNotExist);
                this.RedirectToProfile();
            }

            var model = Mapper.Map<CostEntryUpdateViewModel>(costEntry);
            model.AllCostEntryTypes = await this.GetAllCostEntryTypes();

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Edit(CostEntryUpdateViewModel model)
        {
            var success = await this.costEntries
                .UpdateAsync(model.Id, model.DateCreated, model.CostEntryTypeId, model.VehicleId, model.Price, model.Note, model.Odometer);

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
            var model = await this.costEntries.GetForDeleteAsync(id);

            return View(model);
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Delete(CostEntryDeleteServiceModel model)
        {
            bool success = await this.costEntries.DeleteAsync(model.Id);
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
            var types = await this.costEntries.GetEntryTypesAsync();

            return types.Select(ce => new SelectListItem(ce.Name, ce.Id.ToString())).ToList();
        }
    }
}