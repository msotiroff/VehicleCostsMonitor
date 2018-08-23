namespace VehicleCostsMonitor.Services.Models.User
{
    using System.Collections.Generic;
    using VehicleCostsMonitor.Common.AutoMapping.Interfaces;
    using VehicleCostsMonitor.Models;

    public class UserListingServiceModel : IAutoMapWith<User>
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public ICollection<string> CurrentRoles { get; set; }

        public ICollection<string> NonCurrentRoles { get; set; }
    }
}
