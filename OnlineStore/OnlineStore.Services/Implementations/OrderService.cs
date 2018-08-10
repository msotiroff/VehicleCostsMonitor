namespace OnlineStore.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models.OrderModels;
    using OnlineStore.Models;
    using OnlineStore.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class OrderService : BaseService, IOrderService
    {
        public OrderService(OnlineStoreDbContext db, IMapper mapper)
            : base(db, mapper) { }
        
        public async Task<IEnumerable<OrderViewModel>> GetAsync()
            => await this.DbContext.Orders
                .ProjectTo<OrderViewModel>(this.Mapper.ConfigurationProvider)
                .ToListAsync();
        
        public async Task<OrderViewModel> GetAsync(int id)
        {
            var order = await this.DbContext.Orders.FindAsync(id);

            return this.Mapper.Map<OrderViewModel>(order);
        }
        
        public async Task CreateAsync(OrderCreateServiceModel model)
        {
            if (this.ValidateEntityState(model))
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

                await this.DbContext.Orders.AddAsync(orderToBeCreated);
                await this.DbContext.SaveChangesAsync();
            }
        }
    }
}