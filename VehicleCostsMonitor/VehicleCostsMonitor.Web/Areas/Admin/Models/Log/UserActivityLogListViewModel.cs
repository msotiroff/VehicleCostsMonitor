namespace VehicleCostsMonitor.Web.Areas.Admin.Models.Log
{
    using Infrastructure.Collections;
    using Services.Models.Log;

    public class UserActivityLogListViewModel
    {
        public string SearchTerm { get; set; }

        public PaginatedList<UserActivityLogConciseServiceModel> Logs { get; set; }
    }
}
