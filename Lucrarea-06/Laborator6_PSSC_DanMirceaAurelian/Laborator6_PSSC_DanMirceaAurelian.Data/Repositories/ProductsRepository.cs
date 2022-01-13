using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Laborator6_PSSC_DanMirceaAurelian.Domain.Models.ShoppingCarts;
using static LanguageExt.Prelude;
using Laborator6_PSSC_DanMirceaAurelian.Domain.Models;
using Laborator6_PSSC_DanMirceaAurelian.Domain.Repositories;
using Laborator6_PSSC_DanMirceaAurelian.Data.Models;

namespace Laborator6_PSSC_DanMirceaAurelian.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ShoppingCartsContext shoppingCartsContext;

        public ProductsRepository(ShoppingCartsContext shoppingCartsContext)
        {
            this.shoppingCartsContext = shoppingCartsContext;
        }

        public TryAsync<List<ProductStoc>> TryGetExistingProduct(IEnumerable<string> productsToCheck) => async () =>
        {
            var products = await shoppingCartsContext.Products
                                                .Where(product => productsToCheck.Contains(product.Code))
                                                .AsNoTracking()
                                                .ToListAsync();
            return products.Select(product => new ProductStoc(new ProductCode(product.Code), new Quantity(product.Stoc)))
                            .ToList();
        };
    }
}
