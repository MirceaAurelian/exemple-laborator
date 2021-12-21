using Laborator5_PSSC_DanMirceaAurelian.Domain.Models;
using LanguageExt;
using System.Collections.Generic;
using static Laborator5_PSSC_DanMirceaAurelian.Domain.Models.ShoppingCarts;

namespace Laborator5_PSSC_DanMirceaAurelian.Domain.Repositories
{
    public interface IOrderHeadersRepository
    {
        TryAsync<List<int>> TryGetExistingOrderHeaders(IEnumerable<int> shoppingCartsToCheck);
    }
}
