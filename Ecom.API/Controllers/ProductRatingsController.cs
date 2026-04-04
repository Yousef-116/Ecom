using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class ProductRatingsController : BaseController
    {
        public ProductRatingsController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ratings = await unitOfWork.ProductRatingRepository.GetAllAsync();
                if (ratings == null || !ratings.Any())
                {
                    return NotFound("No ratings found.");
                }

                var ratingsDto = mapper.Map<IEnumerable<ProductRatingDTO>>(ratings);
                return Ok(ratingsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving ratings: {ex.Message}");
            }
        }

        [HttpGet("get-by-product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            try
            {
                var ratings = await unitOfWork.ProductRatingRepository.GetAllAsync();
                var productRatings = ratings.Where(r => r.ProductId == productId).ToList();
                
                if (!productRatings.Any())
                {
                    return NotFound($"No ratings found for product with ID {productId}.");
                }

                var ratingsDto = mapper.Map<IEnumerable<ProductRatingDTO>>(productRatings);
                return Ok(ratingsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving ratings for the product: {ex.Message}");
            }
        }

        [HttpPost("add-rating")]
        public async Task<IActionResult> AddRating([FromBody] AddProductRatingDTO ratingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (ratingDto == null)
                {
                    return BadRequest("Rating data is null.");
                }

                var product = await unitOfWork.ProductRepository.GetByIdAsync(ratingDto.ProductId);
                if (product == null)
                {
                    return NotFound($"Product with ID {ratingDto.ProductId} not found.");
                }

                ProductRating newRating = mapper.Map<ProductRating>(ratingDto);
                await unitOfWork.ProductRatingRepository.AddAsync(newRating);
                
                return Ok("Rating added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the rating: {ex.Message}");
            }
        }
    }
}
