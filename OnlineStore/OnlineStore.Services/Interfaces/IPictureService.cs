using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineStore.Services.Models.PictureModels;

namespace OnlineStore.Services.Interfaces
{
    public interface IPictureService
    {
        Task CreateAsync(PictureCreateServiceModel model);

        Task DeleteAsync(int id);

        Task<PictureViewModel> GetAsync(int id);

        Task<IEnumerable<PictureViewModel>> GetByProductIdAsync(int productId);
    }
}