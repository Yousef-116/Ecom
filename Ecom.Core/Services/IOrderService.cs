using Ecom.Core.DTO;
using Ecom.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Services
{
    public interface IOrderService
    {
        Task<Orders> CreateOrderAsync(AddOrderDTO orderDTO , string BuyerEmail);
        Task<IReadOnlyList<OrderToReturnDTO>>GetAllOrdersforUserAsync(string BuyerEmail);
        Task<OrderToReturnDTO> GetOrderByIdAsync(int Id , string BuyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
    }
}
