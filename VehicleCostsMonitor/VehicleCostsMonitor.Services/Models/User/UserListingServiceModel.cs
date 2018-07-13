namespace VehicleCostsMonitor.Services.Models.User
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Common.AutoMapping;
    using VehicleCostsMonitor.Models;

    public class UserListingServiceModel : IAutoMapWith<User>
    {
        public string Id { get; set; }

        public string Email { get; set; }

        [Display(Name = "Vehicles count")]
        public int VehiclesCount { get; set; }

        public ICollection<string> CurrentRoles { get; set; }

        public ICollection<string> NonCurrentRoles { get; set; }
    }
}
