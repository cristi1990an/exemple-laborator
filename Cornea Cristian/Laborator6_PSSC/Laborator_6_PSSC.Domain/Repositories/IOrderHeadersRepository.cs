
using LanguageExt;
using System.Collections.Generic;

namespace Laborator_6_PSSC.Domain.Repositories
{
    public interface IOrderHeadersRepository
    {
        TryAsync<List<int>> TryGetExistingOrderHeaders(IEnumerable<int> shoppingCartsToCheck);
    }
}
