using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Ecom.infrastructure.Repositories
{
    public class CustomerBasketRepository : ICustomerBasketRepository
    {
        private readonly IDatabase _database;

        public CustomerBasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<bool> DeleteCustomerBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket> GetCustomerBasketAsync(string basketId)
        {
            var result = await _database.StringGetAsync(basketId);
            if (result.IsNullOrEmpty)
            {
                return null;
            }
            return JsonSerializer.Deserialize<CustomerBasket>(result);
        }

        public async Task<CustomerBasket> UpdateCustomerBasketAsync(CustomerBasket basket)
        {
            var serializedBasket = JsonSerializer.Serialize(basket);
            var res = await _database.StringSetAsync(basket.id, serializedBasket, TimeSpan.FromDays(2));
            if (res)
            {
                var updatedBasket = await GetCustomerBasketAsync(basket.id);
                return updatedBasket;  // Return object directly
            }

            return null;
        }

    }
}
