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
            try
            {
            
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var order = await _orderService.CreateOrderAsync(orderDTO, email);
            return Ok(order);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("------------------");
                System.Console.WriteLine(ex.Message);
                return BadRequest(ex.Message); 
            }
        }


        [HttpGet("get-orders-for-user")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDTO>>> getorders()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var order = await _orderService.GetAllOrdersforUserAsync(email);
            return Ok(order);
        }

        [HttpGet("get-order-by-id/{id}")]
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


        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _orderService.UpdateOrderAsync(orderId, dto.Status);

            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }


}
