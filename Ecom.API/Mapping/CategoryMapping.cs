using System;
using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using SQLitePCL;

namespace Ecom.API.Mapping;

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        CreateMap<CategoryDTO, Category>().ReverseMap();

    }

}
