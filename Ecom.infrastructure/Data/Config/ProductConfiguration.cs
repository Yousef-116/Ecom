using Ecom.Core.Entites.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Data.Config
{ 
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.OldPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.NewPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.Id).IsRequired();
            builder.HasData(
                new Product { Id = 1, Name = "Smartphone", Description = "Latest model smartphone with advanced features", OldPrice = 699.99m,NewPrice =100.1m, CategoryId = 1 },
                new Product { Id = 2, Name = "Laptop", Description = "High-performance laptop for work and gaming", OldPrice = 1299.99m,NewPrice =100.1m, CategoryId = 1 },
                new Product { Id = 3, Name = "T-Shirt", Description = "Comfortable cotton t-shirt in various sizes", OldPrice = 19.99m,NewPrice =100.1m, CategoryId = 2 },
                new Product { Id = 4, Name = "Blender", Description = "Powerful blender for smoothies and food preparation", OldPrice = 89.99m,NewPrice =100.1m, CategoryId = 3 }
                );
        }
    }
}
