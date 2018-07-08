namespace OnlineStore.Models
{
    using Common;
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User : IdentityUser
    {
        [Required]
        [MinLength(ModelConstants.CustomerNameMinLength)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(ModelConstants.CustomerNameMinLength)]
        public string LastName { get; set; }

        public IEnumerable<Order> Orders { get; set; } = new HashSet<Order>();

        public IEnumerable<Feedback> SentFeedbacks { get; set; } = new HashSet<Feedback>();
    }
}