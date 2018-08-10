using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineStore.Services.Models.OrderModels;

namespace OnlineStore.Services.Interfaces
{
    public interface IOrderService
    {
        Task CreateAsync(OrderCreateServiceModel model);

        Task<IEnumerable<OrderViewModel>> GetAsync();

        Task<OrderViewModel> GetAsync(int id);
    }
}