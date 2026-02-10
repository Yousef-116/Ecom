using System;
using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;

namespace Ecom.API.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.CategoryName,
            opt => opt.MapFrom(src => src.Category.Name)).ReverseMap();
        //.ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photos));
        CreateMap<Photo, PhotoDTO>().ReverseMap();

        CreateMap<AddProductDTO, Product>()
         .ForMember(p=> p.Photos , op=> op.Ignore())
         .ReverseMap();

        CreateMap<UpdateProductDTO, Product>()
         .ForMember(p => p.Photos, op => op.Ignore())
         .ReverseMap();


    }

}
