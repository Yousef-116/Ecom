using AutoMapper;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class BasketsController : BaseController
    {
        public BasketsController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet("get_basket")]
        public async Task<ActionResult> GetBasket(string id)
        {
            var basket = await unitOfWork.CustomerBasketRepository.GetCustomerBasketAsync(id);
            if (basket == null)
            {
                return Ok(new CustomerBasket(id)); // Return an empty basket if not found
            }
            return Ok(basket);


        }
        [HttpPost("update_basket")]
        public async Task<ActionResult> UpdateBasket([FromBody] CustomerBasket basket)
        {
            var updatedBasket = await unitOfWork.CustomerBasketRepository.UpdateCustomerBasketAsync(basket);
            if (updatedBasket == null)
            {
                return BadRequest("Failed to update the basket.");
            }
            return Ok(updatedBasket);


        }
        [HttpDelete("delete_basket/{id}")]
        public async Task<ActionResult> DeleteBasket(string id)
        {
            var result = await unitOfWork.CustomerBasketRepository.DeleteCustomerBasketAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok("deleted succssfuly");



        }
    }
}
