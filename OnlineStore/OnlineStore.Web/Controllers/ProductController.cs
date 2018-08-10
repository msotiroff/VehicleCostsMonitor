namespace OnlineStore.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OnlineStore.Services.Models.ProductModels;
    using OnlineStore.Common.Notifications;
    using OnlineStore.Web.Infrastructure.Extensions;
    using OnlineStore.Web.Infrastructure.Helpers;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using static WebConstants;
    using OnlineStore.Services.Interfaces;

    public class ProductController : BaseController
    {
        private readonly IFileProcessor fileProcessor;
        private readonly IProductService productService;

        public ProductController(IFileProcessor fileProcessor, IProductService productService)
        {
            this.fileProcessor = fileProcessor;
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var model = await this.GetProductById(id.Value);

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
            await this.productService.CreateAsync(model);
            
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

            var product = await this.GetProductById(id.Value);

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole)]
        public async Task<IActionResult> Edit(ProductUpdateServiceModel model)
        {
            await this.productService.UpdateAsync(model);

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

            await productService.DeleteAsync(id);

            this.ShowNotification(NotificationMessages.ProductDeletedSuccessfull, NotificationType.Info);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        
        private async Task<ProductViewModel> GetProductById(int id)
        {
            var product = await this.productService.GetAsync(id);

            return product;
        }
    }
}