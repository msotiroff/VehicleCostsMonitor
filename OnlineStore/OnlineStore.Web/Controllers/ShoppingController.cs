namespace OnlineStore.Web.Controllers
{
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
    using OnlineStore.Services.Interfaces;
    using Services.Models.OrderModels;
    using Services.Models.ProductModels;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ShoppingController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly ISession session;
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public ShoppingController(
            UserManager<User> userManager,
            IHttpContextAccessor contextAccessor,
            IOrderService orderService,
            IProductService productService,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.orderService = orderService;
            this.productService = productService;
            this.mapper = mapper;
            this.session = contextAccessor.HttpContext.Session;
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

            await this.orderService.CreateAsync(serviceModel);

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
                var dbProduct = await this.productService.GetAsync(productId);

                productInCart = this.mapper.Map<ProductShoppingCartModel>(dbProduct);

                productInCart.Amount++;

                cart.Products
                    .Add(productInCart);
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