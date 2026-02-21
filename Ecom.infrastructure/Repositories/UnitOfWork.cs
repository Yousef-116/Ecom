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
        private readonly IEmailService _emailService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IGenerateToken _generateToken;


        public IProductRepository ProductRepository { get; }

        public ICategoryRepository CategoryRepository { get; }

        public IPhotoRepository PhotoRepository { get; }

        public ICustomerBasketRepository CustomerBasketRepository { get; }

        public IAuth Auth { get; }

        private readonly IConnectionMultiplexer _redis;

        public UnitOfWork(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService, IConnectionMultiplexer redis, UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken generateToken)
        {
            _context = context;
            _mapper = mapper;
            _imageManagementService = imageManagementService;
            _redis = redis;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _generateToken = generateToken;

            ProductRepository = new ProductRepository(_context, _mapper, _imageManagementService);
            CategoryRepository = new CategoryRepository(_context);
            PhotoRepository = new PhotoRepository(_context);
            CustomerBasketRepository = new CustomerBasketRepository(_redis);
            Auth = new AuthRepository(_userManager, _emailService, _signInManager,_generateToken);
        }
    }
}
