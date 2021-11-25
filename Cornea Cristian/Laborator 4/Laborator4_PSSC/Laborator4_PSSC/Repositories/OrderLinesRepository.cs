using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Laborator4_PSSC.Domain.Repositories;
using static Laborator4_PSSC.Domain.ShoppingCarts;
using static LanguageExt.Prelude;
using Laborator4_PSSC.Domain;
using Laborator4_PSSC.Models;

namespace Laborator4_PSSC.Repositories
{
    public class OrderLinesRepository : IOrderLinesRepository
    {
        private readonly ShoppingCartsContext dbContext;

        public OrderLinesRepository(ShoppingCartsContext dbContext)
        {
            this.dbContext = dbContext;
        }
        //CalculatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price, Price finalPrice);
        public TryAsync<List<CalculatedShoppingCart>> TryGetExistingOrderLines() => async () => (await (
                          from ol in dbContext.OrderLines
                          join p in dbContext.Products on ol.OrderId equals p.ProductId
                          join oh in dbContext.OrderHeaders on ol.OrderId equals oh.OrderId
                          select new { oh.OrderId, p.Code, ol.Quantity, oh.Address, ol.Price, oh.Total })
                          .AsNoTracking()
                          .ToListAsync())
                          .Select(result => new CalculatedShoppingCart(
                                                    productCode: new(result.Code),
                                                    quantity: new(result.Quantity),
                                                    address: new(result.Address),
                                                    price: new(result.Price),
                                                    finalPrice: new(result.Total))
                          {
                              OrderId = result.OrderId
                          })
                          .ToList();

        public TryAsync<Unit> TrySaveOrderLines(PaidShoppingCarts shoppingCarts) => async () =>
        {
            var products = (await dbContext.Products.ToListAsync()).ToLookup(product => product.Code);
            var orders = (await dbContext.OrderHeaders.ToListAsync()).ToLookup(order => order.OrderId);
            var newShoppingCarts = shoppingCarts.ShoppingCartList
                                    .Where(sp => sp.IsUpdated && sp.OrderId == 0)
                                    .Select(sp => new OrderLineDto()
                                    {
                                        ProductId = products[sp.productCode.Code].Single().ProductId,
                                        OrderId = orders[sp.OrderId].Single().OrderId,
                                        Quantity = sp.quantity.Value,
                                        Price = sp.price.Value

                                    });
            var updatedShoppingCarts = shoppingCarts.ShoppingCartList.Where(sp => sp.IsUpdated && sp.OrderId > 0)
                                    .Select(sp => new OrderLineDto()
                                    {
                                        OrderId = sp.OrderId,
                                        ProductId = products[sp.productCode.Code].Single().ProductId,
                                        Quantity = sp.quantity.Value,
                                        Price = sp.price.Value
                                    });

            dbContext.AddRange(newShoppingCarts);
            foreach (var entity in updatedShoppingCarts)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return unit;
        };

    }
}
