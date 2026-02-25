using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entites.Product;
using Ecom.Core.Entities.Order;

namespace Ecom.API.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping() {

            CreateMap<Orders,OrderToReturnDTO>()
                .ForMember(x => x.deliveryMethod 
                ,x => x
                .MapFrom(x=> x.deliveryMethod.Name))
                .ReverseMap();

            CreateMap<OrderItem,OrderItemDTO>().ReverseMap();
            CreateMap<ShippingAddress, ShippingAddressDTO>().ReverseMap();

        }

    }
}
