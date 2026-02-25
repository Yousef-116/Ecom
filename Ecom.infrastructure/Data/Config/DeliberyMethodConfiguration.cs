using Ecom.Core.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecom.infrastructure.Data.Config
{
    public class DeliberyMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.Property(m => m.Price).HasColumnType("decimal(18,2)");

            builder.HasData(
                new DeliveryMethod
                {
                    Id = 1,
                    DeleveryTime = "One week",
                    Description = "The fastest ",
                    Name = "Name",
                    Price = 54

                },
                new DeliveryMethod
                {
                     Id = 2,
                     DeleveryTime = "Two week",
                     Description = "more Econmomic delivery",
                     Name = "BaridMAsr",
                     Price = 20

                }
            );
        }
    }
}
