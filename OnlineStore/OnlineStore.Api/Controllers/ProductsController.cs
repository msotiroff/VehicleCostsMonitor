namespace OnlineStore.Api.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OnlineStore.Api.Infrastructure.Filters;
    using OnlineStore.Api.Models.ProductModels;
    using OnlineStore.Data;
    using OnlineStore.Models;
    using System.Linq;
    using System.Threading.Tasks;
    using static ApiConstants;

    public class ProductsController : BaseApiController
    {
        public ProductsController(OnlineStoreDbContext db)
            : base(db) { }

        [HttpGet]
        public async Task<IQueryable<ProductViewModel>> Get()
        {
            var products = await this.db
                .Products
                .ProjectTo<ProductViewModel>()
                .ToListAsync();

            return products.AsQueryable();
        }
        
        [HttpGet(ById)]
        public async Task<ProductViewModel> Get(int id)
        {
            var product = await this.db
                .Products
                .Where(p => p.Id == id)
                .ProjectTo<ProductViewModel>()
                .FirstOrDefaultAsync();

            return product;
        }
        
        [HttpPost]
        [ValidateModelState]
        public async Task Post([FromBody]ProductCreateServiceModel model)
        {
            var productToBeCreated = Mapper.Map<Product>(model);
            
            await this.db.Products.AddAsync(productToBeCreated);
            await this.db.SaveChangesAsync();
        }

        [HttpPut]
        [ValidateModelState]
        public async Task Put([FromBody]ProductUpdateServiceModel model)
        {
            var productToUpdate = await this.db
                .Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (productToUpdate != null)
            {
                productToUpdate = Mapper.Map<Product>(model);

                this.db.Entry(productToUpdate).State = EntityState.Modified;
                await this.db.SaveChangesAsync();
            }
        }

        [HttpDelete(ById)]
        public async Task Delete(int id)
        {
            var productToBeDeleted = await this.db
                .Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productToBeDeleted != null)
            {
                this.db.Products.Remove(productToBeDeleted);
                await this.db.SaveChangesAsync();
            }
        }
    }
}