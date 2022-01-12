using Laborator_6_PSSC.Domain.Models;
using LanguageExt;
using System.Collections.Generic;

namespace Laborator_6_PSSC.Domain.Repositories
{
    public interface IProductsRepository
    {
        TryAsync<List<ProductCode>> TryGetExistingProduct(IEnumerable<string> productsToCheck);
    }
}
