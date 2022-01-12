using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace Laborator_6_PSSC.Domain.Models
{
    [AsChoice]
    public static partial class ShoppingCartsPaidEvent
    {
        public interface IShoppingCartsPaidEvent { }

        public record ShoppingCartsPaidScucceededEvent : IShoppingCartsPaidEvent
        {

            public IEnumerable<PaidShoppingCart> shoppingCarts { get; }
            public DateTime PublishedDate { get; }

            internal ShoppingCartsPaidScucceededEvent(IEnumerable<PaidShoppingCart> shoppingCarts, DateTime publishedDate)
            {
                shoppingCarts = shoppingCarts;
                PublishedDate = publishedDate;
            }
        }

        public record ShoppingCartsPaidFailedEvent : IShoppingCartsPaidEvent
        {
            public string Reason { get; }

            internal ShoppingCartsPaidFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
