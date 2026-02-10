using AutoMapper;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageManagementService _imageManagementService;

        public IProductRepository ProductRepository { get; }

        public ICategoryRepository CategoryRepository { get; }

        public IPhotoRepository PhotoRepository { get; }
        


        public UnitOfWork(AppDbContext context,IMapper mapper , IImageManagementService imageManagementService)
        {
            _context = context;
            _mapper = mapper;
            _imageManagementService = imageManagementService;

            ProductRepository = new ProductRepository(_context,_mapper,_imageManagementService);
            CategoryRepository = new CategoryRepository(_context);
            PhotoRepository = new PhotoRepository(_context);
        }
    }
}
