using Laborator_6_PSSC.Domain.Models;
using LanguageExt;
using System.Collections.Generic;
using static Laborator_6_PSSC.Domain.Models.ShoppingCarts;

namespace Laborator_6_PSSC.Domain.Repositories
{
    public interface IOrderLinesRepository
    {
        TryAsync<List<CalculatedShoppingCart>> TryGetExistingOrderLines();

        TryAsync<Unit> TrySaveOrderLines(PaidShoppingCarts shoppingCarts);
    }
}
