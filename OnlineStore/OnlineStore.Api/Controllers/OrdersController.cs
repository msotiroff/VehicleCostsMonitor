namespace OnlineStore.Api.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.OrderModels;
    using OnlineStore.Api.Infrastructure.Filters;
    using OnlineStore.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static ApiConstants;

    public class OrdersController : BaseApiController
    {
        public OrdersController(OnlineStoreDbContext db)
            : base(db) { }

        [HttpGet]
        public async Task<IEnumerable<OrderViewModel>> Get()
            => await this.db.Orders
                .ProjectTo<OrderViewModel>()
                .ToListAsync();

        [HttpGet(ById)]
        public async Task<OrderViewModel> Get(int id)
        {
            var order = await this.db.Orders.FindAsync(id);

            return Mapper.Map<OrderViewModel>(order);
        }
        
        [HttpPost]
        [ValidateModelState]
        public async Task Post([FromBody]OrderCreateServiceModel model)
        {
            var orderToBeCreated = new Order
            {
                CustomerName = model.CustomerName,
                CustomerPhoneNumber = model.CustomerPhoneNumber,
                DateTime = model.DateTime,
                UserId = model.UserId,
                TotalPrice = model.Products.Sum(p => p.Price * p.Amount),
                Products = model
                    .Products
                    .Select(dict => new ProductState
                    {
                        Name = dict.Name,
                        ProductId = dict.ProductId,
                        Price = dict.Price,
                        OrderedAmount = dict.Amount
                    })
                    .ToArray()
            };
            
            await this.db.Orders.AddAsync(orderToBeCreated);
            await this.db.SaveChangesAsync();
        }
    }
}