using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Laborator4_PSSC.Domain.Repositories;
using static LanguageExt.Prelude;

namespace Laborator4_PSSC.Repositories
{
    public class OrderHeadersRepository : IOrderHeadersRepository
    {
        private readonly ShoppingCartsContext shoppingCartsContext;

        public OrderHeadersRepository(ShoppingCartsContext shoppingCartsContext)
        {
            this.shoppingCartsContext = shoppingCartsContext;
        }

        public TryAsync<List<int>> TryGetExistingOrderHeaders(IEnumerable<int> shoppingCartsToCheck) => async () =>
        {
            var orders = await shoppingCartsContext.OrderHeaders
                                                .Where(order => shoppingCartsToCheck.Contains(order.OrderId))
                                                .AsNoTracking()
                                                .ToListAsync();
            return orders.Select(order => order.OrderId)
                            .ToList();
        };
    }
}
