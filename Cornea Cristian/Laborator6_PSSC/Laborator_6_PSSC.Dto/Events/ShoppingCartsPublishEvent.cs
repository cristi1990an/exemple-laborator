using Laborator_6_PSSC.Dto.Models;
using System.Collections.Generic;

namespace Laborator_6_PSSC.Dto.Events
{
    public record ShoppingCartsPublishEvent
    {
        public List<OrderProductDto> ShoppingCarts { get; init; }
    }
}
