using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator4_PSSC.Domain.ShoppingCarts;


namespace Laborator4_PSSC.Domain.Repositories
{
    public interface IOrderLinesRepository
    {
        TryAsync<List<CalculatedShoppingCart>> TryGetExistingOrderLines();

        TryAsync<Unit> TrySaveOrderLines(PaidShoppingCarts shoppingCarts);
    }
}
