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
        public OrderService(IUnitOfWork unitOfWork, AppDbContext context, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.context = context;
            this.mapper = mapper;
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

            var order = new Orders(
               buyerEmail: BuyerEmail,
               subTotal: subTotal,
               deliveryMethod: deliverMethod,
               orderItems: orderItems,
               shippingAddress: shipping
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
    }
}
