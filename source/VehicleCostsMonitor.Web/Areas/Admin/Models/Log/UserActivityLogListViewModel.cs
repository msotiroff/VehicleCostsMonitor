namespace VehicleCostsMonitor.Web.Areas.Admin.Models.Log
{
    using Infrastructure.Collections;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.Log;
    using System.Collections.Generic;

    public class UserActivityLogListViewModel
    {
        public string SearchTerm { get; set; }

        public IEnumerable<SelectListItem> AllSearchCriterias { get; set; }

        public string SearchCriteria  { get; set; }

        public PaginatedList<UserActivityLogConciseServiceModel> Logs { get; set; }
    }
}
