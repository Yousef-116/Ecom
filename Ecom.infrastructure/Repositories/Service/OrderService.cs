using Ecom.Core.DTO;
using Ecom.Core.Entities;
using Ecom.Core.Entities.Order;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Ecom.infrastructure.Repositories.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IPaymentService paymentService;
        public OrderService(IUnitOfWork unitOfWork, AppDbContext context, IMapper mapper, IPaymentService paymentService)
        {
            this.unitOfWork = unitOfWork;
            this.context = context;
            this.mapper = mapper;
            this.paymentService = paymentService;
        }

        public async Task<Orders> CreateOrderAsync(AddOrderDTO orderDTO, string BuyerEmail)
        {
            CustomerBasket basket = await unitOfWork.CustomerBasketRepository.GetCustomerBasketAsync(orderDTO.basketId);
            List<OrderItem> orderItems = new List<OrderItem>();

            foreach (var item in basket.BasketItems)
            {
                OrderItem orderItem = new OrderItem(
                    price: item.price,
                    quantity: item.quantity,
                    productItemId: item.id,
                    productName: item.Name,
                    mainImage: item.imageName
                    );
                orderItems.Add(orderItem);
            }

            var deliverMethod = context.DeliveryMethods.FirstOrDefault(D => D.Id == orderDTO.DeliveryMethodID);

            var subTotal = orderItems.Sum(x => x.Price * x.Quantity);

            var shipping = mapper.Map<ShippingAddress>(orderDTO.ShippingAddress);

            var ExisitOrder = await context.Orders.Where(o => o.PaymentIntentId == basket.PaymentIntentId).FirstOrDefaultAsync();

            if (ExisitOrder != null)
            {
                context.Orders.Remove(ExisitOrder);
                await paymentService.CreateOrUpdatePaymentAsync(basket.id, deliverMethod.Id);
            }

            var order = new Orders(
               buyerEmail: BuyerEmail,
               subTotal: subTotal,
               deliveryMethod: deliverMethod,
               orderItems: orderItems,
               shippingAddress: shipping,
               paymentIntentId: basket.PaymentIntentId
               );

            await context.AddAsync( order );
            await context.SaveChangesAsync();

            //await unitOfWork.CustomerBasketRepository.DeleteCustomerBasketAsync(orderDTO.basketId);

            return order;
        }

        public async Task<IReadOnlyList<OrderToReturnDTO>> GetAllOrdersforUserAsync(string BuyerEmail)
        {
            var orders = await context.Orders
                .Where(o => o.BuyerEmail == BuyerEmail)
                .Include(o => o.orderItems)
                .Include(o=> o.deliveryMethod)
                .AsNoTracking().ToListAsync();

            var result = mapper.Map<IReadOnlyList<OrderToReturnDTO>>(orders);

            return result;


        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await context.DeliveryMethods.AsNoTracking().ToListAsync();
        }

        public async Task<OrderToReturnDTO?> GetOrderByIdAsync(int Id, string BuyerEmail)
        {
            var ordre = await context.Orders
                .Where(x => x.BuyerEmail == BuyerEmail && x.Id == Id).AsNoTracking()
                .Include(x => x.orderItems)
                .Include(x => x.deliveryMethod)
                .FirstOrDefaultAsync();
            var result = mapper?.Map<OrderToReturnDTO>(ordre);

            return result;
        }
        public async Task<Orders?> UpdateOrderAsync(int orderId, Status status)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return null;

            order.status = status;

            await context.SaveChangesAsync();

            return order;
        }
    }
}
