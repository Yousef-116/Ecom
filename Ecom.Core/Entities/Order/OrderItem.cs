using Ecom.Core.Entites;

namespace Ecom.Core.Entities.Order
{
    public class OrderItem : BaseEntity<int>
    {
        public OrderItem()
        {
            
        }
        public OrderItem(decimal price, int quantity, int productItemId, string productName, string mainImage)
        {
            Price = price;
            Quantity = quantity;
            ProductItemId = productItemId;
            ProductName = productName;
            MainImage = mainImage;
        }
        public decimal Price{ get; set; }
        public int Quantity{ get; set; }
        public int ProductItemId{ get; set; }
        public string ProductName{ get; set; }
        public string MainImage {  get; set; }
    }
}