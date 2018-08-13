namespace VehicleCostsMonitor.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.User;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;

    public class UserService : BaseService, IUserService
    {
        public UserService(JustMonitorDbContext db) 
            : base(db) { }

        public IQueryable<UserListingServiceModel> GetAll()
            => this.db
            .Users
            .OrderBy(u => u.Email)
            .ProjectTo<UserListingServiceModel>();

        public async Task<UserProfileServiceModel> GetAsync(string id)
            => await this.db
            .Users
            .Where(u => u.Id == id)
            .ProjectTo<UserProfileServiceModel>()
            .FirstOrDefaultAsync();

        public async Task<UserProfileServiceModel> GetByEmailAsync(string email)
            => await this.db
            .Users
            .Where(u => u.Email == email)
            .ProjectTo<UserProfileServiceModel>()
            .FirstOrDefaultAsync();
    }
}
