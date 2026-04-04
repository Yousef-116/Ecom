namespace Ecom.Core.DTO
{
    public class ProductRatingDTO
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Message { get; set; }
        public int Score { get; set; }
        public int ProductId { get; set; }
    }
}
