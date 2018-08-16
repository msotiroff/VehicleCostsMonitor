namespace VehicleCostsMonitor.Services.Implementations
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Log;

    public class LogService : BaseService, ILogService
    {
        public LogService(JustMonitorDbContext db) 
            : base(db) { }

        public bool CreateUserActivityLog(UserActivityLogCreateModel model)
        {
            var log = Mapper.Map<UserActivityLog>(model);

            try
            {
                this.ValidateEntityState(log);

                this.db.UserActivityLogs.Add(log);
                this.db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }            
        }

        public IQueryable<UserActivityLogConciseServiceModel> GetAll()
        {
            var logs = this.db
                .UserActivityLogs
                .OrderByDescending(l => l.DateTime)
                .ProjectTo<UserActivityLogConciseServiceModel>();

            return logs;
        }

        public Task<UserActivityLogDetailsServiceModel> GetAsync(int id)
            => this.db
            .UserActivityLogs
            .Where(l => l.Id == id)
            .ProjectTo<UserActivityLogDetailsServiceModel>()
            .FirstOrDefaultAsync();
    }
}
