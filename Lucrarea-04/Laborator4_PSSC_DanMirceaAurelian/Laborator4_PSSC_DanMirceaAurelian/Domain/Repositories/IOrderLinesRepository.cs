using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator4_PSSC_DanMirceaAurelian.Domain.ShoppingCarts;


namespace Laborator4_PSSC_DanMirceaAurelian.Domain.Repositories
{
    public interface IOrderLinesRepository
    {
        TryAsync<List<CalculatedShoppingCart>> TryGetExistingOrderLines();

        TryAsync<Unit> TrySaveOrderLines(PaidShoppingCarts shoppingCarts);
    }
}
