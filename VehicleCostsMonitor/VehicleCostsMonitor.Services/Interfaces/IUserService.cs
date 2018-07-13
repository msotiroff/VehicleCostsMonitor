namespace VehicleCostsMonitor.Services.Interfaces
{
    using Models.User;
    using System.Linq;

    public interface IUserService
    {
        IQueryable<UserListingServiceModel> GetAll();
    }
}
