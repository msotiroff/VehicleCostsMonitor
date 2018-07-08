namespace OnlineStore.Api.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OnlineStore.Api.Infrastructure.Filters;
    using OnlineStore.Api.Models.PictureModels;
    using OnlineStore.Data;
    using OnlineStore.Models;
    using System.Linq;
    using System.Threading.Tasks;
    using static ApiConstants;

    public class PicturesController : BaseApiController
    {
        public PicturesController(OnlineStoreDbContext db)
            : base(db) { }

        [HttpGet(ById)]
        public async Task<PictureViewModel> Get(int id)
        {
            var picture = await this.db
                .Pictures
                .Where(p => p.Id == id)
                .ProjectTo<PictureViewModel>()
                .FirstOrDefaultAsync();

            return picture;
        }

        [HttpGet("{productId}")]
        [Route("GetByProductId/{productId}")]
        public async Task<IQueryable<PictureViewModel>> GetByProductId(int productId)
        {
            var pictures = await this.db
                .Pictures
                .Where(p => p.ProductId == productId)
                .ProjectTo<PictureViewModel>()
                .ToListAsync();

            return pictures.AsQueryable();
        }

        [HttpPost]
        [ValidateModelState]
        public async Task Post([FromBody]PictureCreateServiceModel model)
        {
            var pictureToCreate = Mapper.Map<Picture>(model);
            
            var productId = await this.db
                .Products
                .FirstOrDefaultAsync(pr => pr.Id == model.ProductId);

            if (productId != null)
            {
                await this.db.Pictures.AddAsync(pictureToCreate);
                await this.db.SaveChangesAsync();
            }
        }

        [HttpDelete]
        [Route(ById)]
        public async Task Delete(int id)
        {
            var pictureToBeDeleted = await this.db
                .Pictures
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pictureToBeDeleted != null)
            {
                this.db.Remove(pictureToBeDeleted);
                await this.db.SaveChangesAsync();
            }
        }
    }
}