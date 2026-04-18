using AutoMapper;
using Ecom.Core.DTO;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasketDTO>> GetBasket(string id)
        {
            var basket = await unitOfWork.CustomerBasketRepository.GetCustomerBasketAsync(id);

            // If basket is null, return a new one mapped to DTO
            var mappedBasket = mapper.Map<CustomerBasketDTO>(basket ?? new CustomerBasket(id));

            return Ok(mappedBasket);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerBasketDTO>> UpdateBasket(string id, [FromBody] CustomerBasketDTO basketDto)
        {
            if (basketDto == null)
            {
                return BadRequest(new { Message = "Basket data is null." });
            }

            basketDto.Id = id; // Sync ID from route

            var basketEntity = mapper.Map<CustomerBasket>(basketDto);
            var updatedBasket = await unitOfWork.CustomerBasketRepository.UpdateCustomerBasketAsync(basketEntity);

            if (updatedBasket == null)
            {
                return BadRequest(new { Message = "Failed to update the basket." });
            }

            return Ok(mapper.Map<CustomerBasketDTO>(updatedBasket));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasket(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { Message = "Basket id is required." });

            var exists = await unitOfWork.CustomerBasketRepository
                .GetCustomerBasketAsync(id);

            if (exists == null)
                return NotFound(new { Message = $"Basket with ID {id} not found." });

            await unitOfWork.CustomerBasketRepository
                .DeleteCustomerBasketAsync(id);

            return NoContent();
        }

    }
}
