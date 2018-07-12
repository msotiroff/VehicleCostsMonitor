namespace VehicleCostsMonitor.Services.Models.User
{
    using System.Collections.Generic;
    using VehicleCostsMonitor.Common.AutoMapping;
    using VehicleCostsMonitor.Models;

    public class UserConciseListingModel : IAutoMapWith<User>
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public int VehiclesCount { get; set; }
    }
}
