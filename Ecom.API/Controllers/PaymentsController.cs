using Ecom.Core.Entities;
using Ecom.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        public PaymentsController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }


        [HttpPost("Create")]
        public async Task<ActionResult<CustomerBasket>> create(string basketId, int? deliveryId)
        {
            return await paymentService.CreateOrUpdatePaymentAsync(basketId, deliveryId);
        }
    }
}
