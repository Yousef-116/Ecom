using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
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
                var result = mapper.Map<List<ProductDTO>>(products);
                if (result == null || result.Count() == 0)
                {
                    return NotFound("No products found.");
                }
                else
                {
                    return Ok(result);
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
                var product = await unitOfWork.ProductRepository
                .GetByIdAsync(id, x => x.Category, x => x.Photos);
                var result = mapper.Map<ProductDTO>(product);
                if (result == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the product: {ex.Message}");
            }
        }

        [HttpPost("Add-product")]
        public async Task<IActionResult> addProduct(AddProductDTO productDTO)
        {
            try
            {
                if (productDTO == null)
                {
                    return BadRequest("Product is Null");
                }

                //var newProduct = mapper.Map<Product>(product);

                await unitOfWork.ProductRepository.AddAsync(productDTO);

                return Ok("Product add successfully.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product: {ex.Message}");
            }

        }





    }
}
