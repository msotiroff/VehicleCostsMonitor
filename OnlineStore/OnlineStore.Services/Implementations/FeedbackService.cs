namespace OnlineStore.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using OnlineStore.Data;
    using OnlineStore.Models;
    using OnlineStore.Services.Interfaces;
    using OnlineStore.Services.Models.FeedbackModels;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FeedbackService : BaseService, IFeedbackService
    {
        public FeedbackService(OnlineStoreDbContext db, IMapper mapper) 
            : base(db, mapper)
        {
        }
        
        public async Task<IEnumerable<FeedbackListingViewModel>> GetAllAsync()
        {
            var allFeedbacks = await this.DbContext
                .Feedbacks
                .OrderByDescending(f => f.DateSent)
                .ProjectTo<FeedbackListingViewModel>(this.Mapper.ConfigurationProvider)
                .ToArrayAsync();

            return allFeedbacks;
        }
        
        public async Task<FeedbackDetailsServiceModel> GetAsync(int id)
        {
            var feedback = await this.DbContext.Feedbacks.FindAsync(id);
            
            return feedback != null 
                ? this.Mapper.Map<FeedbackDetailsServiceModel>(feedback) 
                : null;
        }


        public async Task CreateAsync(FeedbackCreateServiceModel model)
        {
            if (!this.ValidateEntityState(model))
            {
                var feedbackToBeCreated = this.Mapper.Map<Feedback>(model);

                await this.DbContext.AddAsync(feedbackToBeCreated);
                await this.DbContext.SaveChangesAsync();
            }
        }
    }
}