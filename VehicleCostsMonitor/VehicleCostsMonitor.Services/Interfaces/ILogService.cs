namespace VehicleCostsMonitor.Services.Interfaces
{
    using Models.Log;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ILogService
    {
        void CreateUserActivityLog(UserActivityLogCreateModel model);

        IQueryable<UserActivityLogConciseServiceModel> GetAll();

        Task<UserActivityLogDetailsServiceModel> GetAsync(int id);
    }
}
