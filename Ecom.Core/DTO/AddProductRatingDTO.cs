using System.ComponentModel.DataAnnotations;

namespace Ecom.Core.DTO
{
    public class AddProductRatingDTO
    {
        [Required]
        public string? Username { get; set; }
        public string? Message { get; set; }
        
        [Required]
        [Range(1, 5, ErrorMessage = "Score must be between 1 and 5")]
        public int Score { get; set; }
        
        [Required]
        public int ProductId { get; set; }
    }
}
