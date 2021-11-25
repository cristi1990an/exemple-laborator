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
    public class ProductsRepository : IProductsRepository
    {
        private readonly ShoppingCartsContext shoppingCartsContext;

        public ProductsRepository(ShoppingCartsContext shoppingCartsContext)
        {
            this.shoppingCartsContext = shoppingCartsContext;
        }

        public TryAsync<List<ProductCode>> TryGetExistingProduct(IEnumerable<string> productsToCheck) => async () =>
        {
            var products = await shoppingCartsContext.Products
                                                .Where(product => productsToCheck.Contains(product.Code))
                                                .AsNoTracking()
                                                .ToListAsync();
            return products.Select(product => new ProductCode(product.Code))
                            .ToList();
        };
    }
}
