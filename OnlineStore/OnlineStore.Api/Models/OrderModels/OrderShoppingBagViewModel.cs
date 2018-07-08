namespace OnlineStore.Api.Models.OrderModels
{
    using OnlineStore.Api.Models.ProductModels;

    public class OrderShoppingBagViewModel
    {
        public ProductViewModel Product { get; set; }

        public int Quantity { get; set; }
    }
}
