namespace OnlineStore.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OnlineStore.Services.Models.CategoryModels;

    public interface ICategoryService
    {
        Task<int> CreateAsync(CategoryCreateServiceModel model);

        Task DeleteAsync(int id);

        Task<IEnumerable<CategoryViewModel>> GetAllAsync();

        Task<CategoryViewModel> GetAsync(int id);
    }
}