namespace OnlineStore.Web.Controllers
{
    using Api.Models.OrderModels;
    using Api.Models.ProductModels;
    using AutoMapper;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.OrderViewModels;
    using Models.ShoppingCartModels;
    using OnlineStore.Common.Notifications;
    using OnlineStore.Models;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ShoppingController : ApiClientController
    {
        private const string RequestUri = "api/Orders/";

        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContext;
        private readonly ISession session;

        public ShoppingController(UserManager<User> userManager, IHttpContextAccessor httpContext)
        {
            this.userManager = userManager;
            this.httpContext = httpContext;
            this.session = this.httpContext.HttpContext.Session;
        }

        [HttpGet]
        public IActionResult Cart()
        {
            var model = this.session
                .GetString(ShoppingCart.CartIdSessionKey)
                .ToShoppingCart()
                .Products;

            return View(model);
        }

        [HttpGet]
        [ValidateReturnUrl]
        public async Task<IActionResult> AddToCart(int productId, string returnUrl)
        {
            if (productId <= 0)
            {
                return BadRequest();
            }

            var success = await this.AddProductToSessionCart(productId);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation, NotificationType.Error);
            }
            else
            {
                this.ShowNotification(NotificationMessages.ProductAddedSuccessfull, NotificationType.Success);
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        [ValidateReturnUrl]
        public IActionResult RemoveFromCart(int productId, string returnUrl)
        {
            if (productId <= 0)
            {
                return BadRequest();
            }
            
            var success = this.RemoveProductFromSessionCart(productId);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation, NotificationType.Error);
            }
            else
            {
                this.ShowNotification(NotificationMessages.ProductRemovedSuccessfull, NotificationType.Success);
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Order()
        {
            var productsInCart = this.session
                .GetString(ShoppingCart.CartIdSessionKey)
                .ToShoppingCart()
                .Products;

            if (!productsInCart.Any())
            {
                return RedirectToAction(nameof(ShoppingController.Cart));
            }

            var model = new OrderCreateViewModel
            {
                Products = productsInCart.Select(p => p.Name),
                TotalPrice = productsInCart.Select(p => p.Amount * p.Price).Sum(),
            };

            if (this.User.Identity.IsAuthenticated)
            {
                var user = await this.userManager.GetUserAsync(User);
                model.CustomerName = $"{user.FirstName} {user.LastName}";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Order(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.ShowNotification(NotificationMessages.MissingData, NotificationType.Error);
                return View();
            }
            
            var productsInCart = this.session
                .GetString(ShoppingCart.CartIdSessionKey)
                .ToShoppingCart()
                .Products;

            var serviceModel = new OrderCreateServiceModel
            {
                CustomerName = model.CustomerName,
                CustomerPhoneNumber = model.CustomerPhoneNumber,
                DateTime = DateTime.UtcNow,
                UserId = this.userManager.GetUserId(User),
                Products = productsInCart
                    .Select(p => new ProductStateServiceModel
                    {
                        Name = p.Name,
                        Price = p.Price,
                        ProductId = p.Id,
                        Amount = p.Amount,
                    })
                    .ToArray()
            };
            
            var postTask = await this.HttpClient.PostAsJsonAsync(RequestUri, serviceModel);
            if (!postTask.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            this.ClearShoppingCart();
            this.ShowNotification(NotificationMessages.OrderSuccess, NotificationType.Success);

            return RedirectToAction(nameof(ShoppingController.Thanks));
        }

        [HttpGet]
        public IActionResult Thanks() => View();

        private async Task<bool> AddProductToSessionCart(int productId)
        {
            var cart = this.session
                            .GetString(ShoppingCart.CartIdSessionKey)
                            .ToShoppingCart();

            var productInCart = cart
                .Products
                .FirstOrDefault(p => p.Id == productId);

            if (productInCart == null)
            {
                var response = await this.HttpClient.GetAsync($"api/Products/{productId}");
                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadAsJsonAsync<ProductViewModel>();
                    var productForCart = Mapper.Map<ProductShoppingCartModel>(product);
                    productForCart.Amount++;

                    cart.Products
                        .Add(productForCart);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                productInCart.Amount++;
            }

            this.session.SetString(ShoppingCart.CartIdSessionKey, cart.ToJson());

            return true;
        }

        private bool RemoveProductFromSessionCart(int productId)
        {
            var cart = this.session
                            .GetString(ShoppingCart.CartIdSessionKey)
                            .ToShoppingCart();

            var productToRemove = cart.Products.FirstOrDefault(p => p.Id == productId);

            if (productToRemove != null)
            {
                productToRemove.Amount = productToRemove.Amount - 1;
                if (productToRemove.Amount < 1)
                {
                    cart.Products.Remove(productToRemove);
                }

                this.session.SetString(ShoppingCart.CartIdSessionKey, cart.ToJson());
                return true;
            }

            return false;
        }

        private void ClearShoppingCart()
            => this.session.SetString(ShoppingCart.CartIdSessionKey, "{}");
    }
}