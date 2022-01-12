using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Laborator_6_PSSC.Domain.Models.ShoppingCarts;
using static LanguageExt.Prelude;
using Laborator_6_PSSC.Domain.Models;
using Laborator_6_PSSC.Domain.Repositories;
using Laborator_6_PSSC.Data.Models;

namespace Laborator_6_PSSC.Data.Repositories
{
    public class OrderLinesRepository : IOrderLinesRepository
    {
        private readonly ShoppingCartsContext dbContext;

        public OrderLinesRepository(ShoppingCartsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //CalculatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price, Price finalPrice);
        public TryAsync<List<CalculatedShoppingCart>> TryGetExistingOrderLines() => async () => ( await (
                           from ol in dbContext.OrderLines
                           join p in dbContext.Products on ol.ProductId equals p.ProductId
                           join oh in dbContext.OrderHeaders on ol.OrderId equals oh.OrderId
                           select new { oh.OrderId, ol.OrderLineId, p.Code, ol.Quantity, oh.Address, ol.Price, oh.Total })
                           .AsNoTracking()
                           .ToListAsync())
                          .Select(result => new CalculatedShoppingCart(
                                                    productCode: new(result.Code),
                                                    quantity: new(result.Quantity),
                                                    address: new(result.Address),
                                                    price: new(result.Price),
                                                    finalPrice: new(result.Total))
                          {
                              OrderId = result.OrderId,
                              OrderLineId = result.OrderLineId
                          })
                          .ToList();

        public TryAsync<Unit> TrySaveOrderLines(PaidShoppingCarts shoppingCarts) => async () =>
        {
            var orders = (await dbContext.OrderHeaders.AsNoTracking().ToListAsync()).ToLookup(order => order.OrderId);
            var products = (await dbContext.Products.AsNoTracking().ToListAsync()).ToLookup(product => product.Code);
            var newOrderHeaders = shoppingCarts.ShoppingCartList
                                    .Where(sp => sp.OrderId == 0)
                                    .Select(oh => new OrderHeaderDto()
                                    {
                                        Address = oh.address._address,
                                        Total = oh.finalPrice.Value
                                    });

            var updatedOrderHeaders = shoppingCarts.ShoppingCartList
                                    .Where(sp => sp.IsUpdated != 0 && sp.OrderId > 0)
                                    .Select(sp => new OrderHeaderDto()
                                    {
                                        OrderId = sp.OrderId,
                                        Address = sp.address._address,
                                        Total = shoppingCarts.ShoppingCartList.Where(s => s.OrderId == sp.OrderId).Select(total => total.quantity.Value * total.price.Value).Sum()
                                    });

            dbContext.AddRange(newOrderHeaders);

            foreach (var entity in updatedOrderHeaders)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            var new_orders =  (await dbContext.OrderHeaders.ToListAsync()).Where(oh => !orders.Any(o => oh.OrderId == o.Key)).ToList();

            var newOrderLines = shoppingCarts.ShoppingCartList
                                .Where(sp => sp.IsUpdated == 2 && sp.OrderId > 0)
                                .Select(sp => new OrderLineDto()
                                {
                                    ProductId = products[sp.productCode.Code].Single().ProductId,
                                    OrderId = orders[sp.OrderId].Single().OrderId,
                                    Quantity = sp.quantity.Value,
                                    Price = sp.price.Value

                                })
                                .Append(shoppingCarts.ShoppingCartList
                                        .Where(sp => sp.IsUpdated == 0 && sp.OrderId == 0)
                                        .Select(sp => new OrderLineDto()
                                        {
                                            ProductId = products[sp.productCode.Code].Single().ProductId,
                                            OrderId = new_orders.Where(oh => oh.Address == sp.address._address && oh.Total == sp.finalPrice.Value).Select(oh => oh.OrderId).FirstOrDefault(),
                                            Quantity = sp.quantity.Value,
                                            Price = sp.price.Value
                                        }));

            var updatedOrderLines = shoppingCarts.ShoppingCartList
                                    .Where(sp => sp.IsUpdated == 1 && sp.OrderId > 0)
                                    .Select(sp => new OrderLineDto()
                                    {
                                        OrderLineId = sp.OrderLineId,
                                        //OrderLineId = order_lines.Where(ol => ol.OrderId == sp.OrderId && ol.ProductId == products[sp.productCode.Code].Single().ProductId).Select(ol => ol.OrderLineId).Single(),
                                        OrderId = orders[sp.OrderId].Single().OrderId,
                                        ProductId = products[sp.productCode.Code].Single().ProductId,
                                        Quantity = sp.quantity.Value,
                                        Price = sp.price.Value
                                    });

            dbContext.AddRange(newOrderLines);

            foreach (var entity in updatedOrderLines)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            var updatedProductStocs = products.Select(p => products[p.Key].Single()).Select(p => new ProductDto()
                                                                        {
                                                                            ProductId = p.ProductId,
                                                                            Code = p.Code,
                                                                            Stoc = p.Stoc - shoppingCarts.ShoppingCartList.Where(sp => sp.productCode.Code == p.Code).Select(sp => sp.quantity.Value).Sum()
                                                                        });

            foreach (var entity in updatedProductStocs)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();



            return unit;
        };

    }
}
