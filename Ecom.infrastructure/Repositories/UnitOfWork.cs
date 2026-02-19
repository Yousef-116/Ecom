using AutoMapper;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageManagementService _imageManagementService;
        private readonly UserManager<AppUser> _userManager;
        

        public IProductRepository ProductRepository { get; }

        public ICategoryRepository CategoryRepository { get; }

        public IPhotoRepository PhotoRepository { get; }

        public ICustomerBasketRepository CustomerBasketRepository { get; }

        public IAuth Auth { get; }

        private readonly IConnectionMultiplexer _redis;

        public UnitOfWork(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService, IConnectionMultiplexer redis, UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _imageManagementService = imageManagementService;
            _redis = redis;
            _userManager = userManager;

            ProductRepository = new ProductRepository(_context, _mapper, _imageManagementService);
            CategoryRepository = new CategoryRepository(_context);
            PhotoRepository = new PhotoRepository(_context);
            CustomerBasketRepository = new CustomerBasketRepository(_redis);
            Auth = new AuthRepository(_userManager);
        }
    }
}
