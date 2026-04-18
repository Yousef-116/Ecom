using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Ecom.API.Controllers
{
    public class ProductRatingsController : BaseController
    {
        private readonly IOutputCacheStore _cacheStore;

        public ProductRatingsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IOutputCacheStore cacheStore
        ) : base(unitOfWork, mapper)
        {
            _cacheStore = cacheStore;
        }

        // ========================= GET ALL =========================
        [HttpGet]
        [OutputCache(Duration = 600, Tags = ["ratings"])]
        public async Task<IActionResult> GetAll()
        {
            var ratings = await unitOfWork.ProductRatingRepository.GetAllAsync();

            if (ratings == null || !ratings.Any())
                return NotFound(new { Message = "No ratings found." });

            var result = mapper.Map<IEnumerable<ProductRatingDTO>>(ratings);
            return Ok(result);
        }

        // ========================= GET BY PRODUCT =========================
        [HttpGet("product/{productId}")]
        [OutputCache(Duration = 600, Tags = ["ratings"])]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            // ✅ FIX: push filtering to DB layer
            var ratings = await unitOfWork.ProductRatingRepository
                .GetByProductIdAsync(productId);

            if (ratings == null || !ratings.Any())
                return NotFound(new { Message = $"No ratings found for product with ID {productId}." });

            var result = mapper.Map<IEnumerable<ProductRatingDTO>>(ratings);
            return Ok(result);
        }

        // ========================= CREATE =========================
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRating([FromBody] AddProductRatingDTO ratingDto)
        {
            if (ratingDto == null)
                return BadRequest(new { Message = "Rating data is null." });

            // Optional but smart validation
            if (ratingDto.Score < 1 || ratingDto.Score > 5)
                return BadRequest(new { Message = "Rating must be between 1 and 5." });

            var product = await unitOfWork.ProductRepository.GetByIdAsync(ratingDto.ProductId);

            if (product == null)
                return NotFound(new { Message = $"Product with ID {ratingDto.ProductId} not found." });

            var newRating = mapper.Map<ProductRating>(ratingDto);

            await unitOfWork.ProductRatingRepository.AddAsync(newRating);

            // ✅ smarter cache eviction
            await _cacheStore.EvictByTagAsync("ratings", default);

            return CreatedAtAction(
                nameof(GetByProductId),
                new { productId = ratingDto.ProductId },
                mapper.Map<ProductRatingDTO>(newRating)
            );
        }
    }
}