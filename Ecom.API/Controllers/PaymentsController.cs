using Ecom.Core.Entities;
using Ecom.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId, int? deliveryId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentAsync(basketId, deliveryId);
            
            if (basket == null)
            {
                return BadRequest(new { Message = "Problem with your basket" });
            }

            return Ok(basket);
        }
    }
}
