using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Ecom.infrastructure.Data;
using Ecom.infrastructure.Repositries;
using Microsoft.EntityFrameworkCore;

namespace Ecom.infrastructure.Repositories
{
    public class ProductRatingRepository : GenericRepository<ProductRating>, IProductRatingRepository
    {
        private readonly AppDbContext _context;

        public ProductRatingRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductRating>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductRatings
                .Where(r => r.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
