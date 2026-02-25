using Ecom.Core.Entites;

namespace Ecom.Core.Entities.Order
{
    public class DeliveryMethod :BaseEntity<int>
    {   
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string DeleveryTime { get; set; }

        public DeliveryMethod()
        {
            
        }
        public DeliveryMethod(string name, decimal price, string description, string deleveryTime)
        {
            Name = name;
            Price = price;
            Description = description;
            DeleveryTime = deleveryTime;
        }

    }
}