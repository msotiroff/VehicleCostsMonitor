namespace OnlineStore.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using OnlineStore.Data;
    using OnlineStore.Models;
    using OnlineStore.Services.Interfaces;
    using OnlineStore.Services.Models.PictureModels;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PictureService : BaseService, IPictureService
    {
        public PictureService(OnlineStoreDbContext db, IMapper mapper)
            : base(db, mapper) { }
        
        public async Task<PictureViewModel> GetAsync(int id)
        {
            var picture = await this.DbContext
                .Pictures
                .Where(p => p.Id == id)
                .ProjectTo<PictureViewModel>(this.Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return picture;
        }
        
        public async Task<IEnumerable<PictureViewModel>> GetByProductIdAsync(int productId)
        {
            var pictures = await this.DbContext
                .Pictures
                .Where(p => p.ProductId == productId)
                .ProjectTo<PictureViewModel>(this.Mapper.ConfigurationProvider)
                .ToListAsync();

            return pictures;
        }
        
        public async Task CreateAsync(PictureCreateServiceModel model)
        {
            if (this.ValidateEntityState(model))
            {
                var pictureToCreate = this.Mapper.Map<Picture>(model);

                var productId = await this.DbContext
                    .Products
                    .FirstOrDefaultAsync(pr => pr.Id == model.ProductId);

                if (productId != null)
                {
                    await this.DbContext.Pictures.AddAsync(pictureToCreate);
                    await this.DbContext.SaveChangesAsync();
                }
            }
        }
        
        public async Task DeleteAsync(int id)
        {
            var pictureToBeDeleted = await this.DbContext
                .Pictures
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pictureToBeDeleted != null)
            {
                this.DbContext.Remove(pictureToBeDeleted);
                await this.DbContext.SaveChangesAsync();
            }
        }
    }
}