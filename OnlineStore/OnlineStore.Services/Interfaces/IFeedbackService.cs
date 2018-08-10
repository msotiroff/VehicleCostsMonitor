using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineStore.Services.Models.FeedbackModels;

namespace OnlineStore.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task CreateAsync(FeedbackCreateServiceModel model);

        Task<IEnumerable<FeedbackListingViewModel>> GetAllAsync();

        Task<FeedbackDetailsServiceModel> GetAsync(int id);
    }
}