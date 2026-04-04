using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecom.Core.Entites.Product
{
    public class ProductRating : BaseEntity<int>
    {
        public string? Username { get; set; }
        public string? Message { get; set; }
        public int Score { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
