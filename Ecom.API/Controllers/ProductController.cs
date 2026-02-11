using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Sharing;
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
        public async Task<IActionResult> Get( [FromQuery] ProductParams productParams)
        {
            var products = await unitOfWork.ProductRepository
                .GetAllAsync(productParams);
            int TotalCount = await unitOfWork.ProductRepository.GetCountAsync();
            return products.Any()
                ? Ok(new Pagination<ProductDTO>(
                    productParams.PageNumber, 
                    productParams.PageSize,
                    TotalCount,
                    products))
                : NotFound("No products found.");
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

        [HttpPut("Update-Product/{id}")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDTO productDTO)
        {
            try
            {
                if (productDTO == null)
                {
                    return BadRequest("Product data is null.");
                }

                await unitOfWork.ProductRepository.UpdateAsync(id, productDTO);

                return Ok("Product updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the Product: {ex.Message}");
            }


        }


        [HttpDelete("Delete-Product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {

            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(id);


                await unitOfWork.ProductRepository.DeleteAsync(product);


                return Ok("Product Deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An Error occurred while Deleting the Product:{ex.Message}");
            }
        }



    }
}
