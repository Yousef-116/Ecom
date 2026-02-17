using Ecom.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface ICustomerBasketRepository
    {
        //Task AddToBasketAsync(int basketId, int productId, int quantity);
        Task<CustomerBasket> GetCustomerBasketAsync(string basketId);
        Task<CustomerBasket> UpdateCustomerBasketAsync(CustomerBasket basket);

        Task<bool> DeleteCustomerBasketAsync(string basketId);
    }
}
