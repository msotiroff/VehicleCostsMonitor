namespace OnlineStore.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OnlineStore.Api.Models.ProductModels;
    using OnlineStore.Common.Notifications;
    using OnlineStore.Web.Infrastructure.Extensions;
    using OnlineStore.Web.Infrastructure.Helpers;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using static WebConstants;

    public class ProductController : ApiClientController
    {
        private const string RequestUri = "api/Products/";

        private readonly IFileProcessor fileProcessor;

        public ProductController(IFileProcessor fileProcessor)
        {
            this.fileProcessor = fileProcessor;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var response = await this.HttpClient.GetAsync(RequestUri);

            var models = await response
                .Content
                .ReadAsJsonAsync<ProductViewModel[]>();

            var model = models
                .FirstOrDefault(p => p.Id == id);

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole)]
        public IActionResult Create(int categoryId)
        {
            var model = new ProductCreateServiceModel() { CategoryId = categoryId };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Create(ProductCreateServiceModel model)
        {
            var postTask = await this.HttpClient.PostAsJsonAsync(RequestUri, model);

            if (!postTask.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            this.ShowNotification(NotificationMessages.ProductCreatedSuccessfull, NotificationType.Info);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            
            var response = await this.HttpClient.GetAsync($"{RequestUri}{id}");

            var product = await response.Content.ReadAsJsonAsync<ProductUpdateServiceModel>();

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Edit(ProductUpdateServiceModel model)
        {
            var putTask = await this.HttpClient.PutAsJsonAsync(RequestUri, model);

            if (!putTask.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            this.ShowNotification(NotificationMessages.ProductUpdatedSuccessfull, NotificationType.Info);

            return RedirectToAction(nameof(Details), new { @id = model.Id });
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            
            var model = await this.GetProductById(id.Value);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole)]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Delete all pictures of this product from file system:
            var productDirectoryPath = $"{WebConstants.ProductPicturesServerPath}{WebConstants.ProductPicturesDirectoryNamePrefix}{id}";
            this.fileProcessor.DeleteDirectory(productDirectoryPath);
            
            var deleteTask = await this.HttpClient.DeleteAsync($"{RequestUri}{id}");

            if (!deleteTask.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            this.ShowNotification(NotificationMessages.ProductDeletedSuccessfull, NotificationType.Info);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        
        private async Task<ProductViewModel> GetProductById(int id)
        {
            var response = await this.HttpClient.GetAsync($"{RequestUri}{id}");

            var product = await response.Content.ReadAsJsonAsync<ProductViewModel>();

            return product;
        }
    }
}