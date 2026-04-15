using Ecom.Core.DTO;
using Ecom.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecom.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddOrderDTO orderDTO)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var order = await _orderService.CreateOrderAsync(orderDTO, email);
            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDTO>>> GetOrders()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetAllOrdersforUserAsync(email);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var order = await _orderService.GetOrderByIdAsync(id, email);
            if (order == null)
            {
                return NotFound(new { Message = $"Order with ID {id} not found." });
            }

            return Ok(order);
        }

        [AllowAnonymous]
        [HttpGet("delivery-methods")]
        public async Task<IActionResult> GetDeliveryMethods()
        {
            var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();
            return Ok(deliveryMethods);
        }

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _orderService.UpdateOrderAsync(orderId, dto.Status);
            if (order == null)
            {
                return NotFound(new { Message = $"Order with ID {orderId} not found." });
            }

            return Ok(order);
        }
    }
}
