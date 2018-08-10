namespace OnlineStore.Services.Models.OrderModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OrderViewModel
    {
        public int Id { get; set; }
        
        public DateTime DateTime { get; set; }

        public string CustomerName { get; set; }
        
        public string CustomerPhoneNumber { get; set; }

        public string UserId { get; set; }

        public IEnumerable<OrderShoppingBagViewModel> OrderProducts { get; set; } = new HashSet<OrderShoppingBagViewModel>();

        public decimal TotalPrice => this.OrderProducts.Sum(po => po.Quantity * po.Product.Price);
    }
}
