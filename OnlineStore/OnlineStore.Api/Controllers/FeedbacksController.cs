namespace OnlineStore.Api.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OnlineStore.Api.Infrastructure.Filters;
    using OnlineStore.Api.Models.FeedbackModels;
    using OnlineStore.Data;
    using OnlineStore.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static ApiConstants;

    public class FeedbacksController : BaseApiController
    {
        public FeedbacksController(OnlineStoreDbContext db) 
            : base(db)
        {
        }

        [HttpGet]
        public async Task<IEnumerable<FeedbackListingViewModel>> Get()
        {
            var allFeedbacks = await this.db
                .Feedbacks
                .OrderByDescending(f => f.DateSent)
                .ProjectTo<FeedbackListingViewModel>()
                .ToArrayAsync();

            return allFeedbacks;
        }

        [HttpGet(ById)]
        public async Task<FeedbackDetailsServiceModel> Get(int id)
        {
            var feedback = await this.db.Feedbacks.FindAsync(id);
            
            return feedback != null 
                ? Mapper.Map<FeedbackDetailsServiceModel>(feedback) 
                : null;
        }

        [HttpPost]
        [ValidateModelState]
        public async Task Post([FromBody]FeedbackCreateServiceModel model)
        {
            var feedbackToBeCreated = Mapper.Map<Feedback>(model);

            await this.db.AddAsync(feedbackToBeCreated);
            await this.db.SaveChangesAsync();
        }
    }
}