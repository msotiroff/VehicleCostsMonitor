namespace VehicleCostsMonitor.Services.Interfaces
{
    using Models.User;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IUserService
    {
        IQueryable<UserListingServiceModel> GetAll();

        Task<UserProfileServiceModel> GetAsync(string id);

        Task<UserProfileServiceModel> GetByEmailAsync(string email);
    }
}
