using Ecom.Core.Entites;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecom.Core.Entities.Order
{
    public class ShippingAddress :BaseEntity<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public ShippingAddress()
        {
            
        }
        public ShippingAddress(string firstName, string lastName, string street, string city, string state, string zipCode)
        {
            FirstName = firstName;
            LastName = lastName;
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }

    }
}