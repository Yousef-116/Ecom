using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.infrastructure.Data;
using Ecom.infrastructure.Repositories;
using Ecom.infrastructure.Repositories.Service;
using Ecom.infrastructure.Repositries;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
namespace Ecom.infrastructure;

public static class infrastructureRegisteration
{
    public static IServiceCollection infrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        //apply DbContext


        services.AddDbContext<AppDbContext>(options => {

            var connectionString = configuration.GetConnectionString("EcomDatabase");
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString) // Auto-detect MySQL version
            );

        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        //apply Unit OF Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        //register email sender
        services.AddScoped<IEmailService, EmailService>();

        //register IOrder Service
        services.AddScoped<IOrderService, OrderService>();

        //services.AddScoped<IRating, RatingRepositry>();
        //register token
        services.AddScoped<IGenerateToken, GenerateToken>();

        //register payment service
        services.AddScoped<IPaymentService, PaymentService>();


        //apply Redis Connectoon
        services.AddSingleton<IConnectionMultiplexer>(i =>
        {
            var config = ConfigurationOptions.Parse(configuration.GetConnectionString("redis"));
            return ConnectionMultiplexer.Connect(config);
        });

        services.AddSingleton<IImageManagementService, ImageManagementService>();
        services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));


        services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(x =>
        {
            x.Cookie.Name = "token";
            x.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Secret"])),
                ValidateIssuer = true,
                ValidIssuer = configuration["Token:Issuer"],
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            x.Events = new JwtBearerEvents
            {

                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["token"];
                    context.Token = token;
                    return Task.CompletedTask;
                }
            };
        });

        //services.AddAuthentication(options =>
        //{
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //})
        //.AddJwtBearer(options =>
        //{
        //    options.RequireHttpsMetadata = false;
        //    options.SaveToken = true;

        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(
        //            Encoding.UTF8.GetBytes(configuration["Token:Secret"])
        //        ),
        //        ValidateIssuer = true,
        //        ValidIssuer = configuration["Token:Issuer"],
        //        ValidateAudience = false,
        //        ClockSkew = TimeSpan.Zero
        //    };

        //    options.Events = new JwtBearerEvents
        //    {
        //        OnMessageReceived = context =>
        //        {
        //            context.Token = context.Request.Cookies["token"];
        //            return Task.CompletedTask;
        //        },
        //        OnAuthenticationFailed = context =>
        //        {
        //            Console.WriteLine("❌ Token failed: " + context.Exception.Message);
        //            return Task.CompletedTask;
        //        },
        //        OnTokenValidated = context =>
        //        {
        //            Console.WriteLine("✅ Token is valid");
        //            return Task.CompletedTask;
        //        }
        //    };
        //});



        return services;
    }
}

