namespace OnlineStore.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using OnlineStore.Web.Infrastructure.Helpers;
    using OnlineStore.Services.Interfaces;
    using System.Linq;
    using System.Threading.Tasks;
    using static WebConstants;
    using OnlineStore.Common.Notifications;
    using OnlineStore.Services.Models.CategoryModels;

    public class CategoryController : BaseController
    {
        private readonly IFileProcessor fileProcessor;
        private readonly IImageProcessor imageProcessor;
        private readonly ICategoryService categoryService;

        public CategoryController(
            IFileProcessor fileProcessor, 
            IImageProcessor imageProcessor,
            ICategoryService categoryService)
        {
            this.fileProcessor = fileProcessor;
            this.imageProcessor = imageProcessor;
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await this.categoryService.GetAllAsync();

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

            var newCategoryId = await this.categoryService.CreateAsync(model);

            if (newCategoryId == default(int))
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

            await this.categoryService.DeleteAsync(id);
            
            this.ShowNotification(NotificationMessages.CategoryDeletedSuccessfull, NotificationType.Info);
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<CategoryViewModel> GetCategoryById(int id)
        {
            var category = await this.categoryService.GetAsync(id);

            return category;
        }
    }
}
