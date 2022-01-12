using Laborator_6_PSSC.Domain.Models;
using static LanguageExt.Prelude;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Laborator_6_PSSC.Domain.Models.ShoppingCarts;
using System.Threading.Tasks;

namespace Laborator_6_PSSC.Domain
{
    public static class ShoppingCartsOperations
    {
        public static Task<IShoppingCarts> ValidateShoppingCarts(Func<ProductCode, Option<ProductCode>> checkProductExists, Func<int, Option<int>> checkOrderHeaderExistsOrInsert, EmptyShoppingCarts shoppingCarts) =>
            shoppingCarts.ShoppingCartList
                      .Select(ValidateShoppingCart(checkProductExists, checkOrderHeaderExistsOrInsert))
                      .Aggregate(CreateEmptyValidatedShoppingCartList().ToAsync(), ReduceValidShoppingCarts)
                      .MatchAsync(
                            Right: validatedShoppingCarts => new ValidatedShoppingCarts(validatedShoppingCarts),
                            LeftAsync: errorMessage => Task.FromResult((IShoppingCarts)new UnvalidatedShoppingCarts(shoppingCarts.ShoppingCartList, new Exception(errorMessage)))
                      );
        private static Func<EmptyShoppingCart, EitherAsync<string, ValidatedShoppingCart>> ValidateShoppingCart(Func<ProductCode, Option<ProductCode>> checkProductExists, Func<int, Option<int>> checkOrderHeaderExistsOrInsert) =>
        emptyShoppingCart => ValidateShoppingCart(checkProductExists, checkOrderHeaderExistsOrInsert, emptyShoppingCart);

        private static EitherAsync<string, ValidatedShoppingCart> ValidateShoppingCart(Func<ProductCode, Option<ProductCode>> checkProductExists, Func<int, Option<int>> checkOrderHeaderExistsOrInsert, EmptyShoppingCart emptyShoppingCart) =>
            from address in Address.TryParse(emptyShoppingCart.address)
                                    .ToEitherAsync(() => $"Invalid address ({emptyShoppingCart.productCode}, {emptyShoppingCart.address})")
            from productCode in ProductCode.TryParse(emptyShoppingCart.productCode)
                                    .ToEitherAsync(() => $"Invalid product code ({emptyShoppingCart.productCode})")
            from productExists in checkProductExists(productCode)
                   .ToEitherAsync($"Product {productCode} does not exist.")
            from orderHeaderExistsOrInsert in checkOrderHeaderExistsOrInsert(emptyShoppingCart.OrderId)
                                    .ToEitherAsync(() => $"Order header ({emptyShoppingCart.OrderId} does not exist.)")
            from quantity in Quantity.TryParse(emptyShoppingCart.quantity)
                                    .ToEitherAsync(() => $"Invalid quantity ({emptyShoppingCart.productCode}, {emptyShoppingCart.quantity})")
            from price in Price.TryParse(emptyShoppingCart.price)
                        .ToEitherAsync(() => $"Invalid price ({emptyShoppingCart.productCode}, {emptyShoppingCart.price})")
            select new ValidatedShoppingCart(productCode, quantity, address, price) { OrderId = emptyShoppingCart.OrderId };

        private static Either<string, List<ValidatedShoppingCart>> CreateEmptyValidatedShoppingCartList() =>
            Right(new List<ValidatedShoppingCart>());

        private static EitherAsync<string, List<ValidatedShoppingCart>> ReduceValidShoppingCarts(EitherAsync<string, List<ValidatedShoppingCart>> acc, EitherAsync<string, ValidatedShoppingCart> next) =>
            from list in acc
            from nextShoppingCart in next
            select list.AppendValidShoppingCart(nextShoppingCart);

        private static List<ValidatedShoppingCart> AppendValidShoppingCart(this List<ValidatedShoppingCart> list, ValidatedShoppingCart validShoppingCart)
        {
            list.Add(validShoppingCart);
            return list;
        }

        public static IShoppingCarts CalculateFinalPrices(IShoppingCarts shoppingCarts) => shoppingCarts.Match(
            whenEmptyShoppingCarts: emptyShoppingCart => emptyShoppingCart,
            whenUnvalidatedShoppingCarts: unvalidatedShoppingCart => unvalidatedShoppingCart,
            whenCalculatedShoppingCarts: calculatedShoppingCart => calculatedShoppingCart,
            whenPaidShoppingCarts: paidShoppingCart => paidShoppingCart,
            whenValidatedShoppingCarts: CalculateFinalPrice
        );

        private static IShoppingCarts CalculateFinalPrice(ValidatedShoppingCarts validShoppingCarts) =>
            new CalculatedShoppingCarts(validShoppingCarts.ShoppingCartList
                                                          .Select(CalculateShoppingCartFinalPrice)
                                                          .ToList()
                                                          .AsReadOnly());
        private static CalculatedShoppingCart CalculateShoppingCartFinalPrice(ValidatedShoppingCart validShoppingCart) =>
            new CalculatedShoppingCart(validShoppingCart.productCode,
                                      validShoppingCart.quantity,
                                      validShoppingCart.address,
                                      validShoppingCart.price,
                                      validShoppingCart.price * validShoppingCart.quantity) { OrderId = validShoppingCart.OrderId };


        public static IShoppingCarts MergeShoppingCarts(IShoppingCarts shoppingCarts, IEnumerable<CalculatedShoppingCart> existingShoppingCarts) => shoppingCarts.Match(
            whenEmptyShoppingCarts: emptyShoppingCarts => emptyShoppingCarts,
            whenUnvalidatedShoppingCarts: unvalidatedShoppingCarts => unvalidatedShoppingCarts,
            whenValidatedShoppingCarts: validatedShoppingCarts => validatedShoppingCarts,
            whenPaidShoppingCarts: paidShoppingCarts => paidShoppingCarts,
            whenCalculatedShoppingCarts: calculatedShoppingCarts => MergeShoppingCarts(calculatedShoppingCarts.ShoppingCartList, existingShoppingCarts));

        private static CalculatedShoppingCarts MergeShoppingCarts(IEnumerable<CalculatedShoppingCart> newList, IEnumerable<CalculatedShoppingCart> existingList)
        {
            var updatedAndNewShoppingCarts = newList.Select(sp => sp with { IsUpdated = 1, OrderLineId = existingList.FirstOrDefault(s => sp.OrderId == s.OrderId && sp.productCode == s.productCode)?.OrderLineId ?? 0 }).Where(sp => existingList.Any(s => sp.OrderId == s.OrderId && sp.productCode == s.productCode))
                                                    .Append(newList.Where(sp => sp.OrderId == 0))
                                                    .Append(newList.Select(sp => sp with { IsUpdated = 2 }).Where(sp => !existingList.Any(s => sp.OrderId == s.OrderId && sp.productCode == s.productCode) && existingList.Any(s => s.OrderId == sp.OrderId))) ;

            var oldShoppingCarts = existingList.Where(sp => !newList.Any(s => sp.OrderId == s.OrderId && sp.productCode == s.productCode));
            var allShoppingCarts = updatedAndNewShoppingCarts.Union(oldShoppingCarts)
                                               .ToList()
                                               .AsReadOnly();
            return new CalculatedShoppingCarts(allShoppingCarts);
        }
        public static IShoppingCarts PayShoppingCarts(IShoppingCarts shoppingCarts) => shoppingCarts.Match(
            whenEmptyShoppingCarts: emptyShoppingCart => emptyShoppingCart,
            whenUnvalidatedShoppingCarts: unvalidatedShoppingCart => unvalidatedShoppingCart,
            whenPaidShoppingCarts: paidShoppingCart => paidShoppingCart,
            whenValidatedShoppingCarts: validatedShoppingCart => validatedShoppingCart,
            whenCalculatedShoppingCarts: GenerateExport
        );

        private static IShoppingCarts GenerateExport(CalculatedShoppingCarts calculatedShoppingCart) =>
            new PaidShoppingCarts(calculatedShoppingCart.ShoppingCartList,
                                    calculatedShoppingCart.ShoppingCartList.Aggregate(new StringBuilder(), CreateCsvLine).ToString(),
                                    DateTime.Now);

        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedShoppingCart shoppingCart) =>
            export.AppendLine($"{shoppingCart.productCode.Code}, {shoppingCart.price}, {shoppingCart.quantity}, {shoppingCart.finalPrice}");
    }
}
