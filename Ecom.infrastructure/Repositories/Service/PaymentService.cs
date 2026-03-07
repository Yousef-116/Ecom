using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;
        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration, AppDbContext context)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
            this.context = context;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId,int ? deliverMethod)
        {
            var basket = await unitOfWork.CustomerBasketRepository.GetCustomerBasketAsync(basketId);
            StripeConfiguration.ApiKey = configuration["StripeSetting:secretKey"];
            decimal shippingAmount = 0m;
            if (deliverMethod.HasValue)
            {
                var delivery = await context.DeliveryMethods.AsNoTracking().FirstOrDefaultAsync(m => m.Id == deliverMethod.Value);
                shippingAmount = delivery.Price;
            }
            foreach (var item in basket.BasketItems)
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(item.id);
                item.price = product.NewPrice;
            }

            PaymentIntentService paymentIntentService = new();
            PaymentIntent paymentIntent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var option = new PaymentIntentCreateOptions
                {
                    Amount = (long) (basket.BasketItems.Sum(m => (m.quantity * m.price * 100)) + (shippingAmount * 100)),
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card"}
                };
                paymentIntent = await paymentIntentService.CreateAsync(option);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;

            }
            else
            {
                var option = new PaymentIntentUpdateOptions
                {
                    Amount = (long)(basket.BasketItems.Sum(m => (m.quantity * m.price * 100)) + (shippingAmount * 100)),
                };
                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, option);
            }
            await unitOfWork.CustomerBasketRepository.UpdateCustomerBasketAsync(basket);
            return basket;

        }
    }
}
