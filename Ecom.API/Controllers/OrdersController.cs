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

        private string? GetUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }

        // ========================= CREATE =========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddOrderDTO orderDTO)
        {
            var email = GetUserEmail();
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var order = await _orderService.CreateOrderAsync(orderDTO, email);

            if (order == null)
                return BadRequest(new { Message = "Problem creating order." });

            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = order.Id },
                order
            );
        }

        // ========================= GET ALL =========================
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDTO>>> GetOrders()
        {
            var email = GetUserEmail();
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var orders = await _orderService.GetAllOrdersforUserAsync(email);

            return Ok(orders);
        }

        // ========================= GET BY ID =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var email = GetUserEmail();
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var order = await _orderService.GetOrderByIdAsync(id, email);

            if (order == null)
                return NotFound(new { Message = $"Order with ID {id} not found." });

            return Ok(order);
        }

        // ========================= DELIVERY METHODS =========================
        [AllowAnonymous]
        [HttpGet("delivery-methods")]
        public async Task<IActionResult> GetDeliveryMethods()
        {
            var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();

            return Ok(deliveryMethods);
        }

        // ========================= UPDATE STATUS =========================
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId,
            [FromBody] UpdateOrderStatusDto dto)
        {
            if (dto == null)
                return BadRequest(new { Message = "Invalid status data." });

            var order = await _orderService.UpdateOrderAsync(orderId, dto.Status);

            if (order == null)
                return NotFound(new { Message = $"Order with ID {orderId} not found." });

            return NoContent(); // or NoContent() if you don't need data
            //return Ok(order); // or NoContent() if you don't need data
        }
    }
}