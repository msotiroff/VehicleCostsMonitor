namespace OnlineStore.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using OnlineStore.Api.Models.PictureModels;
    using OnlineStore.Common.Notifications;
    using OnlineStore.Web.Infrastructure.Extensions;
    using OnlineStore.Web.Infrastructure.Helpers;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using static WebConstants;

    [Authorize(Roles = AdministratorRole)]
    public class PictureController : ApiClientController
    {
        private const string RequestUri = "api/Pictures/";

        private readonly IFileProcessor fileProcessor;
        private readonly IImageProcessor imageProcessor;

        public PictureController(
            IFileProcessor fileProcessor,
            IImageProcessor imageProcessor)
        {
            this.fileProcessor = fileProcessor;
            this.imageProcessor = imageProcessor;
        }

        [HttpPost]
        public async Task<IActionResult> Add(List<IFormFile> files, int id)
        {
            var uploadDirectory = this.fileProcessor.EnsureDirectoryExist(id);

            var imageTasks = new List<Task>();

            foreach (var file in files)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var isFileExtensionValid = this.imageProcessor.ValidateImageExtenssion(fileExtension);

                if (!isFileExtensionValid)
                {
                    continue;
                }

                var uniqueFileName = this.fileProcessor.GetUniqueFileName(file.FileName);
                var postTask = await this.SavePictureToDatabase(id, uniqueFileName);

                if (!postTask.IsSuccessStatusCode)
                {
                    continue;
                }

                var task = this.SavePictureToServer(uploadDirectory, file, uniqueFileName);
                imageTasks.Add(task);
            }

            await Task.WhenAll(imageTasks);

            this.ShowNotification(NotificationMessages.PicturesUploadedSuccessfull, NotificationType.Info);

            return RedirectToAction(nameof(ProductController.Details), "Product", new { id });
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await this.HttpClient.GetAsync(RequestUri + id);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            var pictureToBeDeleted = response.Content.ReadAsJsonAsync<PictureViewModel>().Result;

            // Removes the picture from file system:
            var successfullyDeleted = this.fileProcessor.DeleteFile(pictureToBeDeleted.Path);

            if (!successfullyDeleted)
            {
                return BadRequest();
            }

            this.ShowNotification(NotificationMessages.PictureDeletedSuccessfull, NotificationType.Info);

            return RedirectToAction(nameof(ProductController.Details), "Product", new { @id = pictureToBeDeleted.ProductId });
        }

        private async Task SavePictureToServer(string uploadDirectory, IFormFile file, string uniqueFileName)
        {
            var fullPath = this.fileProcessor.GetFullPath(uploadDirectory, uniqueFileName);

            await this.fileProcessor
                .SaveToFullPath(fullPath, file);

            this.imageProcessor.ProcessImage(fullPath);
        }

        private async Task<HttpResponseMessage> SavePictureToDatabase(int productId, string uniqueFileName)
        {
            // Add the current picture to database
            var dbPath = "\\" + Path.Combine(WebConstants.ProductPicturesServerPath,
                WebConstants.ProductPicturesDirectoryNamePrefix + productId.ToString(),
                uniqueFileName);

            var serviceModel = new PictureCreateServiceModel
            {
                Path = dbPath,
                ProductId = productId
            };

            var postTask = await this.HttpClient.PostAsJsonAsync(RequestUri, serviceModel);
            return postTask;
        }
    }
}