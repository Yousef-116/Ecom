using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;

namespace Ecom.API.Mapping
{
    public class ProductRatingMapping : Profile
    {
        public ProductRatingMapping()
        {
            CreateMap<ProductRating, ProductRatingDTO>().ReverseMap();
            CreateMap<AddProductRatingDTO, ProductRating>();
        }
    }
}
