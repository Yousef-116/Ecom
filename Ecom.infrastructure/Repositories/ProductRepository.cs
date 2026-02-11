using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.infrastructure.Data;
using Ecom.infrastructure.Repositries;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IImageManagementService imageManagementService;
        public ProductRepository(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageManagementService = imageManagementService;
        }

        public async Task<List<ProductDTO>> GetAllAsync(string sort,int? categroyId)
        {
            var query = context.Products
                .Include(p => p.Category)
                .Include(p => p.Photos)
                .AsNoTracking();

            if(categroyId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categroyId.Value);
            }
            if (query != null)
            {
                switch (sort)
                {
                    case "PriceAsn":
                        query = query.OrderBy(p => p.NewPrice);

                        break;
                    case "PriceDes":
                        query = query.OrderByDescending(p => p.NewPrice);

                        break;
                    default:
                        query = query.OrderBy(p => p.Name);

                        break;

                }
            }
            
            var result = mapper.Map<List<ProductDTO>>(query);


            return result;
        }
        public async Task<bool> AddAsync(AddProductDTO productDTO)
        {
            if (productDTO == null) return false;

            var product = mapper.Map<Product>(productDTO);

            await context.Products.AddAsync(product);

            await context.SaveChangesAsync(); // Exeption

            var imagePaths = await imageManagementService.AddImageAsync(productDTO.Photo, productDTO.Name);

            var photos = imagePaths
             .Select(path => new Photo { 
                 ImageName = path ,ProductId = product.Id
             });

            await context.Photos.AddRangeAsync(photos);
            await context.SaveChangesAsync();

            return true;

        }

        public async Task DeleteAsync(Product product)
        {
            var photos = await context.Photos.Where(ph => ph.ProductId == product.Id).ToListAsync();
            foreach (var photo in photos)
            {
                imageManagementService.DeleteImageAsync(photo.ImageName);
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();

        }

        public async Task<bool> UpdateAsync(int id ,UpdateProductDTO productDTO)
        {
            if (productDTO == null) return false;

            var UpdateProduct = await context.Products
                .Include(m => m.Category)
                .Include(p=> p.Photos)
                .FirstOrDefaultAsync(pro => pro.Id == id); 

            if(UpdateProduct == null) return false;

            mapper.Map(productDTO, UpdateProduct);

            //await context.Products.Update(product);
            var photos = await context.Photos.Where(pho => pho.ProductId == id).ToListAsync();

            foreach (var photo in photos)
            {
                imageManagementService.DeleteImageAsync(photo.ImageName);
            }
            context.Photos.RemoveRange(photos);

            var imagePaths = await imageManagementService
                .AddImageAsync(productDTO.Photo, productDTO.Name);

            var newphotos = imagePaths
            .Select(path => new Photo
            {
                ImageName = path,
                ProductId = id
            });


            await context.Photos.AddRangeAsync(newphotos);
            await context.SaveChangesAsync();

            return true;


        }
    }
}
