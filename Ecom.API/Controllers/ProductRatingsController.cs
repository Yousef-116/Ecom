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

        public ProductRatingsController(IUnitOfWork unitOfWork, IMapper mapper, IOutputCacheStore cacheStore) 
            : base(unitOfWork, mapper)
        {
            _cacheStore = cacheStore;
        }

        [HttpGet]
        [OutputCache(Duration = 600, Tags = ["ratings"])]
        public async Task<IActionResult> GetAll()
        {
            var ratings = await unitOfWork.ProductRatingRepository.GetAllAsync();
            if (ratings == null || !ratings.Any())
            {
                return NotFound(new { Message = "No ratings found." });
            }

            var ratingsDto = mapper.Map<IEnumerable<ProductRatingDTO>>(ratings);
            return Ok(ratingsDto);
        }

        [HttpGet("product/{productId}")]
        [OutputCache(Duration = 600, Tags = ["ratings"])]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var ratings = await unitOfWork.ProductRatingRepository.GetAllAsync();
            var productRatings = ratings.Where(r => r.ProductId == productId).ToList();
            
            if (!productRatings.Any())
            {
                return NotFound(new { Message = $"No ratings found for product with ID {productId}." });
            }

            var ratingsDto = mapper.Map<IEnumerable<ProductRatingDTO>>(productRatings);
            return Ok(ratingsDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRating([FromBody] AddProductRatingDTO ratingDto)
        {
            if (ratingDto == null)
            {
                return BadRequest(new { Message = "Rating data is null." });
            }

            var product = await unitOfWork.ProductRepository.GetByIdAsync(ratingDto.ProductId);
            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {ratingDto.ProductId} not found." });
            }

            var newRating = mapper.Map<ProductRating>(ratingDto);
            await unitOfWork.ProductRatingRepository.AddAsync(newRating);

            // Evict the cached ratings so that the next GET request fetches fresh data
            await _cacheStore.EvictByTagAsync("ratings", default);
            
            return Ok(new { Message = "Rating added successfully." });
        }
    }
}
