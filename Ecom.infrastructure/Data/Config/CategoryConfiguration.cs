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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x=> x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Id).IsRequired();
            builder.HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets" },
                new Category { Id = 2, Name = "Clothing", Description = "Apparel and fashion items" },
                new Category { Id = 3, Name = "Home & Kitchen", Description = "Household items and kitchenware" },
                new Category { Id = 4, Name = "Books", Description = "Books and literature" },
                new Category { Id = 5, Name = "Sports & Outdoors", Description = "Sporting goods and outdoor equipment" }
            );

        }
    }
}
