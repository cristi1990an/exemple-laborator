using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Laborator_6_PSSC.Domain.Models;
using Laborator_6_PSSC.Domain.Repositories;

namespace Laborator_6_PSSC.Data.Repositories
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
