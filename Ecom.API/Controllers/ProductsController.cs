using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Sharing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Ecom.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet]
        [OutputCache(Duration = 600)]
        public async Task<IActionResult> Get([FromQuery] ProductParams productParams)
        {
            var products = await unitOfWork.ProductRepository.GetAllAsync(productParams);

            return Ok(new Pagination<ProductDTO>(
                productParams.PageNumber,
                productParams.PageSize,
                products.TotalCount,
                products.Products));
        }

        [HttpGet("{id}")]
        [OutputCache(Duration = 600)]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id, x => x.Category, x => x.Photos);

            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found." });
            }

            var result = mapper.Map<ProductDTO>(product);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductDTO productDTO)
        {
            if (productDTO == null)
            {
                return BadRequest(new { Message = "Product data is null." });
            }

            await unitOfWork.ProductRepository.AddAsync(productDTO);
            return Ok(new { Message = "Product added successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDTO productDTO)
        {
            if (productDTO == null)
            {
                return BadRequest(new { Message = "Product data is null." });
            }

            await unitOfWork.ProductRepository.UpdateAsync(id, productDTO);
            return Ok(new { Message = "Product updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found." });
            }

            await unitOfWork.ProductRepository.DeleteAsync(product);
            return Ok(new { Message = "Product deleted successfully." });
        }
    }
}
