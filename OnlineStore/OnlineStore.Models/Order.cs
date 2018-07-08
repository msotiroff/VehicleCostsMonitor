namespace OnlineStore.Models
{
    using Common;
    using OnlineStore.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [MinLength(ModelConstants.CustomerNameMinLength)]
        public string CustomerName { get; set; }

        [Required]
        [Phone]
        public string CustomerPhoneNumber { get; set; }

        public OrderState State { get; set; } = OrderState.Waiting;

        [Required]
        public decimal TotalPrice { get; set; }
        
        public string UserId { get; set; }

        public User User { get; set; }
        
        public IEnumerable<ProductState> Products { get; set; } = new HashSet<ProductState>();
    }
}