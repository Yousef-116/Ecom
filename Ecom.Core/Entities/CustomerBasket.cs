using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Entities
{
    public class CustomerBasket
    {

        public CustomerBasket()
        {
            
        }
        public CustomerBasket(string id)
        {
            this.id = id;
        }
        public string id { get; set; }
        public List<BasketItem> basketItem { get; set; } = new List<BasketItem>();

    }
}
