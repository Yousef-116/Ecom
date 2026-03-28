
using Ecom.Core.Entites;

namespace Ecom.Core.Entities.Order
{
    public class Orders :BaseEntity<int>
    {
        public Orders()
        {

        }
        public Orders(string buyerEmail, decimal subTotal, ShippingAddress shippingAddress, DeliveryMethod deliveryMethod, IReadOnlyList<OrderItem> orderItems, string paymentIntentId = null)
        {
            BuyerEmail = buyerEmail;
            SubTotal = subTotal;
            this.shippingAddress = shippingAddress;
            this.deliveryMethod = deliveryMethod;
            this.orderItems = orderItems;
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }

        public string PaymentIntentId { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public ShippingAddress shippingAddress { get; set; }
        public DeliveryMethod deliveryMethod { get; set; }
        public IReadOnlyList<OrderItem> orderItems{ get; set; }
        public Status status { get; set; } = Status.Pending;

        public decimal GetTotal()
        {
            return SubTotal + deliveryMethod.Price;
        }

    }
}
