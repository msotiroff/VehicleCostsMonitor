namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Linq;
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
        [EnsureOwnership]
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
        [EnsureOwnership]
        public async Task<IActionResult> Update(IFormFile file, int oldPictureId, int vehicleId)
        {
            // If no file chosen => redirect to form:
            if (file == null)
            {
                this.ShowNotification(NotificationMessages.NoFileChosen, NotificationType.Warning);
                return RedirectToAction(nameof(Update), new { vehicleId });
            }

            // If file is too large => redirect to form:
            if (file.Length > WebConstants.PictureSizeLimit)
            {
                this.ShowNotification(string.Format(NotificationMessages.FileIsTooLarge, (WebConstants.PictureSizeLimit / 1024 / 1024)));
                return RedirectToAction(nameof(Update), new { vehicleId });
            }
            
            // Process the file to the file system:
            var extension = this.GetValidExtension(file.FileName);
            if (extension == null)
            {
                this.ShowNotification(NotificationMessages.InvalidFileFormat);
                return RedirectToAction(nameof(this.Update), new { vehicleId });
            }

            var dbPath = string.Format(WebConstants.VehicleImagePathBase, this.GetUniqueFileName(vehicleId, extension));
            
            var fileProcessingSuccess = this.ProcessFile(file, dbPath);
            
            if (fileProcessingSuccess)
            {
                // After successfull processing the image to file system => save it to database:
                var success = await this.pictures.CreateAsync(dbPath, vehicleId);
                if (success)
                {
                    // After success processing new picture, the old one should be deleted from file system and database:
                    await this.Delete(oldPictureId);
                    this.ShowNotification(NotificationMessages.PictureUpdatedSuccessfull, NotificationType.Success);
                }
                else
                {
                    // If writing the picture to database fails => redirect to form:
                    this.ShowNotification(NotificationMessages.PictureUploadFailed);
                    return RedirectToAction(nameof(this.Update), new { vehicleId });
                }
            }

            return RedirectToVehicle(vehicleId);
        }

        [HttpPost]
        [EnsureOwnership]
        public async Task<IActionResult> Delete(int oldPictureId, int vehicleId)
        {
            await this.Delete(oldPictureId);

            return RedirectToVehicle(vehicleId);
        }

        #region Private methods

        private bool ProcessFile(IFormFile file, string dbPath)
        {
            var fileFullPath = Path.Combine(this.webRootPath, dbPath.TrimStart('/', '\\'));

            var uploaded = this.UploadImage(fileFullPath, file);
            if (!uploaded)
            {
                return false;
            }

            bool resized = this.ResizeImage(fileFullPath);
            if (!resized)
            {
                return false;
            }

            return true;
        }

        private bool ResizeImage(string fileFullPath)
        {
            var pictureMaxHeight = WebConstants.PictureMaxHeightSize;

            Image resizedImage = null;

            using (var imageOriginal = Image.FromFile(fileFullPath))
            {
                // Height will increase the width proportionally:
                int width = (pictureMaxHeight * imageOriginal.Width) / imageOriginal.Height;
                int height = pictureMaxHeight;

                resizedImage = new Bitmap(width, height);

                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImage(imageOriginal, 0, 0, width, height);
                }
            }

            if (resizedImage != null)
            {
                resizedImage.Save(fileFullPath);
                return true;
            }

            return false;
        }

        private bool UploadImage(string fileFullPath, IFormFile file)
        {
            try
            {
                using (var stream = new FileStream(fileFullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetValidExtension(string fileName)
        {
            var extension = fileName.Split('.').LastOrDefault()?.ToLower();

            switch (extension)
            {
                case "jpg":
                case "png":
                case "jpeg":
                case "bmp":
                    return extension;
                default:
                    return null;
            }
        }

        private string GetUniqueFileName(int vehicleId, string extension)
        {
            return $"ID_{vehicleId}_{Guid.NewGuid().ToString().Substring(0, 6)}.{extension}";
        }

        private async Task Delete(int oldPictureId)
        {
            if (oldPictureId != default(int))
            {
                // Remove from file system:
                var localPath = await this.pictures.GetPathAsync(oldPictureId);
                var fileFullPath = Path.Combine(this.webRootPath, localPath.TrimStart('/', '\\'));
                if (System.IO.File.Exists(fileFullPath))
                {
                    System.IO.File.Delete(fileFullPath);
                }

                // Remove from db:
                await this.pictures.DeleteAsync(oldPictureId);
            }
        }

        #endregion
    }
}