namespace OnlineStore.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Models.ShoppingCartModels;
    using Newtonsoft.Json;
    using System;

    public static class ShoppingCartExtensions
    {
        public static ShoppingCart GetCart(IServiceProvider services)
        {
            var session = services
                .GetRequiredService<IHttpContextAccessor>()
                ?.HttpContext
                .Session;

            var cartId = session.GetString(ShoppingCart.CartIdSessionKey) ?? Guid.NewGuid().ToString();

            session.SetString(ShoppingCart.CartIdSessionKey, cartId);

            var cart = new ShoppingCart();

            return cart;
        }

        public static ShoppingCart ToShoppingCart(this string shoppingCartAsJson)
        {
            var cart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartAsJson ?? "{}");

            return cart ?? new ShoppingCart();
        }

        public static string ToJson(this ShoppingCart shoppingCart)
        {
            var cartAsJson = JsonConvert.SerializeObject(shoppingCart);

            return cartAsJson;
        }
    }
}
