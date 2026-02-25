using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Core.Sharing;
using Ecom.infrastructure.Data;
using Ecom.infrastructure.Repositries;
using Microsoft.EntityFrameworkCore;


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

        public async Task<ReturnProductListDTO> GetAllAsync( ProductParams productParams)
        {
            
            var query = context.Products
                .Include(p => p.Category)
                .Include(p => p.Photos)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(productParams.Search))
            {
                var words = productParams.Search
                    .ToLower()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    query = query.Where(p =>
                        p.Name.ToLower().Contains(word) ||
                        p.Description.ToLower().Contains(word));
                }
            }



            if (productParams.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == productParams.CategoryId.Value);

            query = productParams.Sort switch
            {
                "PriceAsn" => query.OrderBy(p => p.NewPrice),
                "PriceDes" => query.OrderByDescending(p => p.NewPrice),
                _ => query.OrderBy(p => p.Name)
            };

            //var totalCount = await query.CountAsync();
            ReturnProductListDTO productListDTO = new ReturnProductListDTO();
            productListDTO.TotalCount = await query.CountAsync();

            var products = await query
                .Skip((productParams.PageNumber - 1) * productParams.PageSize)
                .Take(productParams.PageSize)
                .ToListAsync();

            productListDTO.Products = mapper.Map<List<ProductDTO>>(products);

            return productListDTO;

        }

        public async Task<bool> AddAsync(AddProductDTO productDTO)
        {
            if (productDTO == null) return false;

            var product = mapper.Map<Product>(productDTO);

            await context.Products.AddAsync(product);

            await context.SaveChangesAsync(); // Exeption

            var imagePaths = await imageManagementService.AddImageAsync(productDTO.Photo, productDTO.Name);

            var photos = imagePaths
             .Select(path => new Photo
             {
                 ImageName = path,
                 ProductId = product.Id
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

        public async Task<bool> UpdateAsync(int id, UpdateProductDTO productDTO)
        {
            if (productDTO == null) return false;

            var UpdateProduct = await context.Products
                .Include(m => m.Category)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(pro => pro.Id == id);

            if (UpdateProduct == null) return false;

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
