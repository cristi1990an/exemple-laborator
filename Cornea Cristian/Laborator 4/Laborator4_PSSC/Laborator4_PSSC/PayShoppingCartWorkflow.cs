using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Laborator4_PSSC.Domain.ShoppingCartsPaidEvent;
using static Laborator4_PSSC.Domain.ShoppingCarts;
using static Laborator4_PSSC.Domain.ShoppingCartsOperations;
using Laborator4_PSSC.Domain.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace Laborator4_PSSC.Domain
{
    public class PayShoppingCartWorkflow
    {
        private readonly IProductsRepository productsRepository;
        private readonly IOrderHeadersRepository orderHeadersRepository;
        private readonly IOrderLinesRepository orderLinesRepository;
        private readonly ILogger<PayShoppingCartWorkflow> logger;

        public PayShoppingCartWorkflow(IProductsRepository productsRepository, IOrderHeadersRepository orderHeadersRepository, IOrderLinesRepository orderLinesRepository, ILogger<PayShoppingCartWorkflow> logger)
        {
            this.productsRepository = productsRepository;
            this.orderHeadersRepository = orderHeadersRepository;
            this.orderLinesRepository = orderLinesRepository;
            this.logger = logger;
        }
        public async Task<IShoppingCartsPaidEvent> ExecuteAsync(PayShoppingCartCommand command)
        {
            EmptyShoppingCarts emptyShoppingCarts = new EmptyShoppingCarts(command.InputShoppingCarts);
            var result = from products in productsRepository.TryGetExistingProduct(emptyShoppingCarts.ShoppingCartList.Select(sp => sp.productCode))
                                          .ToEither(ex => new UnvalidatedShoppingCarts(emptyShoppingCarts.ShoppingCartList, ex) as IShoppingCarts)
                         from existingOrderLines in orderLinesRepository.TryGetExistingOrderLines()
                                          .ToEither(ex => new UnvalidatedShoppingCarts(emptyShoppingCarts.ShoppingCartList, ex) as IShoppingCarts)
                         let checkProductExists = (Func<ProductCode, Option<ProductCode>>)(product => CheckProductExists(products, product))
                         from paidShoppingCarts in ExecuteWorkflowAsync(emptyShoppingCarts, existingOrderLines, checkProductExists).ToAsync()
                         from _ in orderLinesRepository.TrySaveOrderLines(paidShoppingCarts)
                                          .ToEither(ex => new UnvalidatedShoppingCarts(emptyShoppingCarts.ShoppingCartList, ex) as IShoppingCarts)
                         select paidShoppingCarts;

            return await result.Match(
                    Left: shoppingCarts => GenerateFailedEvent(shoppingCarts) as IShoppingCartsPaidEvent,
                    Right: paidShoppingCarts => new ShoppingCartsPaidScucceededEvent(paidShoppingCarts.Csv, paidShoppingCarts.PublishedDate)
                );
        }
        private async Task<Either<IShoppingCarts, PaidShoppingCarts>> ExecuteWorkflowAsync
            (EmptyShoppingCarts emptyShoppingCarts, IEnumerable<CalculatedShoppingCart> existingOrderLines, Func<ProductCode, Option<ProductCode>> checkProductExists)
        {
            IShoppingCarts shoppingCarts = await ValidateShoppingCarts(checkProductExists, emptyShoppingCarts);
            shoppingCarts = CalculateFinalPrices(shoppingCarts);
            shoppingCarts = MergeGrades(shoppingCarts, existingOrderLines);
            shoppingCarts = PayShoppingCarts(shoppingCarts);

            return shoppingCarts.Match<Either<IShoppingCarts, PaidShoppingCarts>>(
                whenEmptyShoppingCarts: emptyShoppingCarts => Left(emptyShoppingCarts as IShoppingCarts),
                whenCalculatedShoppingCarts: calculatedShoppingCarts => Left(calculatedShoppingCarts as IShoppingCarts),
                whenUnvalidatedShoppingCarts: unvalidatedShoppingCarts => Left(unvalidatedShoppingCarts as IShoppingCarts),
                whenValidatedShoppingCarts: validatedShoppingCarts => Left(validatedShoppingCarts as IShoppingCarts),
                whenPaidShoppingCarts: paidShoppingCarts => Right(paidShoppingCarts)
            );
        }

        private Option<ProductCode> CheckProductExists(IEnumerable<ProductCode> products, ProductCode productCode)
        {
            if (products.Any(p => p == productCode))
            {
                return Some(productCode);
            }
            else
            {
                return None;
            }
        }

        private ShoppingCartsPaidFailedEvent GenerateFailedEvent(IShoppingCarts shoppingCarts) =>
            shoppingCarts.Match<ShoppingCartsPaidFailedEvent>
            (
                whenEmptyShoppingCarts: emptyShoppingCarts => new($"Invalid state {nameof(EmptyShoppingCarts)}"),
                whenValidatedShoppingCarts: validatedShoppingCarts => new($"Invalid state {nameof(ValidatedShoppingCarts)}"),
                whenUnvalidatedShoppingCarts: unvalidatedShoppingCarts =>
                {
                    logger.LogError(unvalidatedShoppingCarts.Ex, unvalidatedShoppingCarts.Ex.Message);
                    return new(unvalidatedShoppingCarts.Ex.Message);
                },
                whenCalculatedShoppingCarts: calculatedExamGrades => new($"Invalid state {nameof(CalculatedShoppingCarts)}"),
                whenPaidShoppingCarts: publishedExamGrades => new($"Invalid state {nameof(PaidShoppingCarts)}")
            );
    }

}
