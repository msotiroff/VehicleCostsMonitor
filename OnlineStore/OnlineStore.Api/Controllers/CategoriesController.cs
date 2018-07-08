namespace OnlineStore.Api.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OnlineStore.Api.Infrastructure.Filters;
    using OnlineStore.Api.Models.CategoryModels;
    using OnlineStore.Data;
    using OnlineStore.Models;
    using System.Linq;
    using System.Threading.Tasks;
    using static ApiConstants;

    public class CategoriesController : BaseApiController
    {
        public CategoriesController(OnlineStoreDbContext db) 
            : base(db) { }

        [HttpGet]
        public async Task<IQueryable<CategoryViewModel>> Get()
        {
            var allCategories = await this.db
                .Categories
                .ProjectTo<CategoryViewModel>()
                .ToListAsync();

            return allCategories.AsQueryable();
        }

        [HttpGet(ById)]
        public async Task<CategoryViewModel> Get(int id)
        {
            var category = await this.db
                .Categories
                .Where(c => c.Id == id)
                .ProjectTo<CategoryViewModel>()
                .FirstOrDefaultAsync(c => c.Id == id);

            return category;
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<int> Post([FromBody]CategoryCreateServiceModel model)
        {
            var categoryToCreate = Mapper.Map<Category>(model);
            
            await this.db.Categories.AddAsync(categoryToCreate);
            await this.db.SaveChangesAsync();

            return categoryToCreate.Id;
        }

        [HttpDelete(ById)]
        public async Task Delete(int id)
        {
            var categoryToBeDeleted = await this.db
                .Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoryToBeDeleted != null)
            {
                this.db.Remove(categoryToBeDeleted);
                await this.db.SaveChangesAsync();
            }
        }
    }
}