namespace OnlineStore.Web.Models.ShoppingCartModels
{
    using System.Collections.Generic;

    public class ShoppingCart
    {
        public const string CartIdSessionKey = "SHOPPING_CART_ID";

        public ShoppingCart()
        {
            this.Products = new HashSet<ProductShoppingCartModel>();
        }

        public ICollection<ProductShoppingCartModel> Products { get; set; }
    }
}
