namespace OnlineStore.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using OnlineStore.Data;
    using OnlineStore.Models;
    using OnlineStore.Services.Interfaces;
    using OnlineStore.Services.Models.CategoryModels;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoryService : BaseService, ICategoryService
    {
        public CategoryService(OnlineStoreDbContext db, IMapper mapper) 
            : base(db, mapper) { }
        
        public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
        {
            var allCategories = await this.DbContext
                .Categories
                .ProjectTo<CategoryViewModel>(this.Mapper.ConfigurationProvider)
                .ToListAsync();

            return allCategories;
        }
        
        public async Task<CategoryViewModel> GetAsync(int id)
        {
            var category = await this.DbContext
                .Categories
                .Where(c => c.Id == id)
                .ProjectTo<CategoryViewModel>(this.Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(c => c.Id == id);

            return category;
        }
        
        public async Task<int> CreateAsync(CategoryCreateServiceModel model)
        {
            if (!this.ValidateEntityState(model))
            {
                return default(int);
            }

            var categoryToCreate = this.Mapper.Map<Category>(model);
            
            await this.DbContext.Categories.AddAsync(categoryToCreate);
            await this.DbContext.SaveChangesAsync();

            return categoryToCreate.Id;
        }
        
        public async Task DeleteAsync(int id)
        {
            var categoryToBeDeleted = await this.DbContext
                .Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoryToBeDeleted != null)
            {
                this.DbContext.Remove(categoryToBeDeleted);
                await this.DbContext.SaveChangesAsync();
            }
        }
    }
}