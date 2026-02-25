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

        [HttpPost("create-order")]
        public async Task<IActionResult> Create(AddOrderDTO orderDTO)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var order = await _orderService.CreateOrderAsync(orderDTO, email);
            return Ok(order);
        }

        [HttpGet("GetOrdersForUser")]
        public async Task<IActionResult> GetOrdersForUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var orders = await _orderService.GetAllOrdersforUserAsync(email);
            return Ok(orders);
        }

        [HttpGet("GetOrderById/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var order = await _orderService.GetOrderByIdAsync(id, email);

            if (order is null)
                return NotFound();

            return Ok(order);
        }

        [AllowAnonymous]
        [HttpGet("delivery-methods")]
        public async Task<IActionResult> GetDeliveryMethods()
            => Ok(await _orderService.GetDeliveryMethodsAsync());
    }
}
