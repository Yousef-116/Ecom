using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Ecom.infrastructure.Data;
using Ecom.infrastructure.Repositries;

namespace Ecom.infrastructure.Repositories
{
    public class ProductRatingRepository : GenericRepository<ProductRating>, IProductRatingRepository
    {
        private readonly AppDbContext _context;

        public ProductRatingRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
