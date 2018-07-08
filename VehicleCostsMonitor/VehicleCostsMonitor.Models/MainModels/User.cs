using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace VehicleCostsMonitor.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.Vehicles = new HashSet<Vehicle>();
        }

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
