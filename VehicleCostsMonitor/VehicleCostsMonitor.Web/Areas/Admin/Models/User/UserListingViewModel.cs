namespace VehicleCostsMonitor.Web.Areas.Admin.Models.User
{
    using Infrastructure.Collections;
    using Services.Models.User;

    public class UserListingViewModel
    {
        public string SearchTerm { get; set; }
        
        public PaginatedList<UserListingServiceModel> Users { get; set; }
    }
}
