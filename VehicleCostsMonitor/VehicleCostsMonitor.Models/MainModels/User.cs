namespace VehicleCostsMonitor.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;

    public class User : IdentityUser
    {
        public User()
        {
            this.Vehicles = new HashSet<Vehicle>();
        }

        public int? CurrencyId { get; set; }

        public Currency DisplayCurrency { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
