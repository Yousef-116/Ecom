using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.infrastructure.Data;
using Ecom.infrastructure.Repositories;
using Ecom.infrastructure.Repositories.Service;
using Ecom.infrastructure.Repositries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;

namespace Ecom.infrastructure
{
    public static class infrastructureRegisteration
    {
        public static IServiceCollection infrastructureConfiguration(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //emial sender
            services.AddScoped<IEmailService, EmailService>();

            //token generator
            services.AddScoped<IGenerateToken, GenerateToken>();

            //apply redis
            services.AddSingleton<IConnectionMultiplexer>(i =>
            {
                var configurationOptions = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"));
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            services.AddSingleton<IImageManagementService, ImageManagementService>();

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot")));

            services.AddDbContext<AppDbContext>(options => {

                var connectionString = configuration.GetConnectionString("EcomDatabase");
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString) // Auto-detect MySQL version
                );

            });
            return services;
        }
    }
}
