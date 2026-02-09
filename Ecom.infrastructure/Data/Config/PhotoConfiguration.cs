using System;
using Ecom.Core.Entites.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecom.infrastructure.Data.Config;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasData(
            new Photo { Id = 1, ImageName = "https://example.com/photo1.jpg", ProductId = 1 },
            new Photo { Id = 2, ImageName = "https://example.com/photo2.jpg", ProductId = 1 },
            new Photo { Id = 3, ImageName = "https://example.com/photo3.jpg", ProductId = 2 },
            new Photo { Id = 4, ImageName = "https://example.com/photo4.jpg", ProductId = 3 },
            new Photo { Id = 5, ImageName = "https://example.com/photo5.jpg", ProductId = 4 }
        );

    }
}
