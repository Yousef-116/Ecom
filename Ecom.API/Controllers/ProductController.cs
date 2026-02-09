using AutoMapper;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{

    public class ProductController : BaseController
    {
        public ProductController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> get()
        {
            try
            {
                var products = await unitOfWork.ProductRepository
                .GetAllAsync(x => x.Category, x => x.Photos);
                if (products == null || products.Count() == 0)
                {
                    return NotFound("No products found.");
                }
                else
                {
                    return Ok(products);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving products: {ex.Message}");
            }
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> getById(int id)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }
                else
                {
                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the product: {ex.Message}");
            }
        }



    }
}
