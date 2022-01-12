using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Laborator_6_PSSC.Domain.Models.ShoppingCarts;
using static LanguageExt.Prelude;
using Laborator_6_PSSC.Domain.Models;
using Laborator_6_PSSC.Domain.Repositories;
using Laborator_6_PSSC.Data.Models;

namespace Laborator_6_PSSC.Data.Repositories
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
            var inserts = shoppingCartsToCheck.Where(orderID => orderID == 0);
            var orders = await shoppingCartsContext.OrderHeaders
                                                .Where(order => shoppingCartsToCheck.Contains(order.OrderId))
                                                .AsNoTracking()
                                                .ToListAsync();
            return orders.Select(order => order.OrderId).Append(inserts)
                            .ToList();
        };
    }
}
