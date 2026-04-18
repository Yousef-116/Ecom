using Ecom.Core.Entites.Product;

namespace Ecom.Core.Interfaces
{
    public interface IProductRatingRepository : IGenericRepository<ProductRating>
    {
        Task<List<ProductRating>> GetByProductIdAsync(int productId);
    }
}
