namespace OnlineStore.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using OnlineStore.Data;
    using OnlineStore.Models;
    using OnlineStore.Services.Interfaces;
    using OnlineStore.Services.Models.ProductModels;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProductService : BaseService, IProductService
    {
        public ProductService(OnlineStoreDbContext db, IMapper mapper)
            : base(db, mapper) { }
        
        public async Task<IEnumerable<ProductViewModel>> GetAsync()
        {
            var products = await this.DbContext
                .Products
                .ProjectTo<ProductViewModel>(this.Mapper.ConfigurationProvider)
                .ToListAsync();

            return products;
        }
        
        public async Task<ProductViewModel> GetAsync(int id)
        {
            var product = await this.DbContext
                .Products
                .Where(p => p.Id == id)
                .ProjectTo<ProductViewModel>(this.Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return product;
        }
        
        public async Task CreateAsync(ProductCreateServiceModel model)
        {
            if (this.ValidateEntityState(model))
            {
                var productToBeCreated = this.Mapper.Map<Product>(model);

                await this.DbContext.Products.AddAsync(productToBeCreated);
                await this.DbContext.SaveChangesAsync();
            }
        }
        
        public async Task UpdateAsync(ProductUpdateServiceModel model)
        {
            if (this.ValidateEntityState(model))
            {
                var productToUpdate = await this.DbContext
                .Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == model.Id);

                if (productToUpdate != null)
                {
                    productToUpdate = this.Mapper.Map<Product>(model);

                    this.DbContext.Entry(productToUpdate).State = EntityState.Modified;
                    await this.DbContext.SaveChangesAsync();
                }
            }
        }
        
        public async Task DeleteAsync(int id)
        {
            var productToBeDeleted = await this.DbContext
                .Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productToBeDeleted != null)
            {
                this.DbContext.Products.Remove(productToBeDeleted);
                await this.DbContext.SaveChangesAsync();
            }
        }
    }
}