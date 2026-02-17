namespace Ecom.Core.Entities
{
    public class BasketItem
    {
        public int id { get; set; }
        public string Name { get; set; }

        public string imageName { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }

        public string categoryName { get; set; }
    }
}