namespace OnlineStore.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using OnlineStore.Api.Models.CategoryModels;
    using OnlineStore.Common.Notifications;
    using OnlineStore.Web.Infrastructure.Extensions;
    using OnlineStore.Web.Infrastructure.Helpers;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using static WebConstants;

    public class CategoryController : ApiClientController
    {
        private const string RequestUri = "api/Categories/";

        private readonly IFileProcessor fileProcessor;
        private readonly IImageProcessor imageProcessor;

        public CategoryController(IFileProcessor fileProcessor, IImageProcessor imageProcessor)
        {
            this.fileProcessor = fileProcessor;
            this.imageProcessor = imageProcessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await this.HttpClient.GetAsync(RequestUri);
            
            var jsonCategories = await response.Content.ReadAsStringAsync();
            
            var categories = JsonConvert.DeserializeObject<IEnumerable<CategoryViewModel>>(jsonCategories);

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            CategoryViewModel category = await this.GetCategoryById(id);

            return View(category);
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole)]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Create(string categoryName, IFormFile image)
        {
            var model = new CategoryCreateServiceModel
            {
                Name = categoryName,
                Thumbnail = this.imageProcessor.GetBytesFromImage(image),
            };

            var postTask = await this.HttpClient.PostAsJsonAsync(RequestUri, model);

            if (!postTask.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            this.ShowNotification(NotificationMessages.CategoryCreatedSuccessfull, NotificationType.Info);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Delete(int id)
        {
            CategoryViewModel category = await this.GetCategoryById(id);
            
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole)]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Delete all pictures of products in this category:
            this.GetCategoryById(id)
                .Result
                .Products
                .Select(pr => pr.Id)
                .ToList()
                .ForEach(productId => this
                    .fileProcessor
                    .DeleteDirectory($"{WebConstants.ProductPicturesServerPath}{WebConstants.ProductPicturesDirectoryNamePrefix}{productId}"));

            var deleteTask = await this.HttpClient.DeleteAsync(RequestUri + id);

            if (!deleteTask.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            this.ShowNotification(NotificationMessages.CategoryDeletedSuccessfull, NotificationType.Info);
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<CategoryViewModel> GetCategoryById(int id)
        {
            var response = await this.HttpClient.GetAsync($"{RequestUri}{id}");

            var category = await response.Content.ReadAsJsonAsync<CategoryViewModel>();

            return category;
        }
    }
}
