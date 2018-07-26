namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Picture;
    using VehicleCostsMonitor.Web.Infrastructure.Filters;

    [Authorize]
    public class PictureController : BaseVehicleController
    {
        private readonly string webRootPath;
        private readonly IPictureService pictures;

        public PictureController(
            IPictureService pictures,
            IHostingEnvironment environment)
        {
            this.pictures = pictures;
            this.webRootPath = environment.WebRootPath;
        }

        [HttpGet]
        [AuthorizeOwner]
        public async Task<IActionResult> Update(int vehicleId)
        {
            var model = await this.pictures
                .GetByVehicleId(vehicleId)
                ?? new PictureUpdateServiceModel
                {
                    Path = GlobalConstants.DefaultPicturePath,
                    VehicleId = vehicleId
                };

            return View(model);
        }

        [HttpPost]
        [AuthorizeOwner]
        public async Task<IActionResult> Update(IFormFile file, int oldPictureId, int vehicleId)
        {
            // If no file chosen => redirect to form:
            if (file == null)
            {
                this.ShowNotification(NotificationMessages.NoFileChosen, NotificationType.Warning);
                return RedirectToAction(nameof(Update), new { vehicleId });
            }

            // TODO: Process the file to the file system:



            // After successfull processing the image to file system => save it to database:
            var success = await this.pictures.CreateAsync("some path", vehicleId);
            if (success)
            {
                // After success processing new picture, the old one should be deleted:
                await this.pictures.DeleteAsync(oldPictureId);

                this.ShowNotification(NotificationMessages.PictureUpdatedSuccessfull, NotificationType.Success);
            }
            else
            {
                // If writing the picture to database fails => redirect to form:
                this.ShowNotification(NotificationMessages.PictureUploadFailed);
                return RedirectToAction(nameof(this.Update), new { vehicleId });
            }

            return LocalRedirect($"/vehicle/details/{vehicleId}");
        }
    }
}