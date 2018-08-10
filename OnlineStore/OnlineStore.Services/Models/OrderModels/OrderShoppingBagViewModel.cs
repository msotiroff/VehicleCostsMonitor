namespace OnlineStore.Services.Models.OrderModels
{
    using OnlineStore.Services.Models.ProductModels;

    public class OrderShoppingBagViewModel
    {
        public ProductViewModel Product { get; set; }

        public int Quantity { get; set; }
    }
}
