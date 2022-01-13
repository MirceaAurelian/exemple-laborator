using Laborator6_PSSC_DanMirceaAurelian.Domain.Models;
using LanguageExt;
using System.Collections.Generic;
using static Laborator6_PSSC_DanMirceaAurelian.Domain.Models.ShoppingCarts;

namespace Laborator6_PSSC_DanMirceaAurelian.Domain.Repositories
{
    public interface IOrderLinesRepository
    {
        TryAsync<List<CalculatedShoppingCart>> TryGetExistingOrderLines();

        TryAsync<Unit> TrySaveOrderLines(PaidShoppingCarts shoppingCarts);
    }
}
