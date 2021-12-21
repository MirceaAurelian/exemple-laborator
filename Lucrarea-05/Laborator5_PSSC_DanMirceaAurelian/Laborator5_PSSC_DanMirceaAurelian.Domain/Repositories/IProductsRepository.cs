using Laborator5_PSSC_DanMirceaAurelian.Domain.Models;
using LanguageExt;
using System.Collections.Generic;
using static Laborator5_PSSC_DanMirceaAurelian.Domain.Models.ShoppingCarts;

namespace Laborator5_PSSC_DanMirceaAurelian.Domain.Repositories
{
    public interface IProductsRepository
    {
        TryAsync<List<ProductCode>> TryGetExistingProduct(IEnumerable<string> productsToCheck);
    }
}
