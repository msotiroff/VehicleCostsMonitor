namespace VehicleCostsMonitor.Web.Areas.Admin.Models.User
{
    using Infrastructure.Collections;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.User;
    using System.Collections.Generic;

    public class UserListingViewModel
    {
        public string SearchTerm { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        public PaginatedList<UserConciseListingModel> Users { get; set; }
    }
}
