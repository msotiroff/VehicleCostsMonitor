namespace VehicleCostsMonitor.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Models.User;
    using System.Linq;
    using VehicleCostsMonitor.Data;

    public class UserService : DataAccessService, IUserService
    {
        public UserService(JustMonitorDbContext db) 
            : base(db) { }

        public IQueryable<UserConciseListingModel> GetAll()
            => this.db
            .Users
            .OrderBy(u => u.Email)
            .ProjectTo<UserConciseListingModel>();
    }
}
