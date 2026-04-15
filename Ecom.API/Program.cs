using Ecom.API.Middleware;
using Ecom.infrastructure;
using Ecom.infrastructure.Data;

namespace Ecom.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.infrastructureConfiguration(builder.Configuration);
            builder.Services.AddMemoryCache();
            builder.Services.AddControllers();
            builder.Services.AddOutputCache();

            // Swagger setup
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));

            // CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CORSPolicy",
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });



            var app = builder.Build();

            // Seed test data in development
            if (app.Environment.IsDevelopment())
            {
                await DataSeeder.SeedAsync(app.Services);
            }


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("CORSPolicy");

            // Enable authentication and authorization
            app.UseOutputCache();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseStaticFiles();
            app.MapControllers();

            app.Run();
        }
    }
}
