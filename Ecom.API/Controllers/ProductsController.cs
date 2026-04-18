using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Sharing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Ecom.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        // ========================= GET ALL =========================
        [HttpGet]
        [OutputCache(Duration = 600)]
        public async Task<IActionResult> Get([FromQuery] ProductParams productParams)
        {
            var products = await unitOfWork.ProductRepository.GetAllAsync(productParams);

            var result = new Pagination<ProductDTO>(
                productParams.PageNumber,
                productParams.PageSize,
                products.TotalCount,
                products.Products
            );

            return Ok(result);
        }

        // ========================= GET BY ID =========================
        [HttpGet("{id}")]
        [OutputCache(Duration = 600)]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await unitOfWork.ProductRepository
                .GetByIdAsync(id, x => x.Category, x => x.Photos);

            if (product == null)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            var result = mapper.Map<ProductDTO>(product);
            return Ok(result);
        }

        // ========================= CREATE =========================
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDTO productDTO)
        {
            if (productDTO == null)
                return BadRequest(new { Message = "Product data is null." });

            var createdProduct = await unitOfWork.ProductRepository.AddAsync(productDTO);

            // IMPORTANT: repository should return created entity with Id
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                mapper.Map<ProductDTO>(createdProduct)
            );
        }

        // ========================= UPDATE =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDTO productDTO)
        {
            if (productDTO == null)
                return BadRequest(new { Message = "Product data is null." });

            var existingProduct = await unitOfWork.ProductRepository.GetByIdAsync(id);

            if (existingProduct == null)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            await unitOfWork.ProductRepository.UpdateAsync(id, productDTO);

            return NoContent();
        }

        // ========================= DELETE =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            await unitOfWork.ProductRepository.DeleteAsync(product);

            return NoContent();
        }
    }
}