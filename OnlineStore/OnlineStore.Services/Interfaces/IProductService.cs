using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineStore.Services.Models.ProductModels;

namespace OnlineStore.Services.Interfaces
{
    public interface IProductService
    {
        Task CreateAsync(ProductCreateServiceModel model);

        Task DeleteAsync(int id);

        Task<IEnumerable<ProductViewModel>> GetAsync();

        Task<ProductViewModel> GetAsync(int id);

        Task UpdateAsync(ProductUpdateServiceModel model);
    }
}