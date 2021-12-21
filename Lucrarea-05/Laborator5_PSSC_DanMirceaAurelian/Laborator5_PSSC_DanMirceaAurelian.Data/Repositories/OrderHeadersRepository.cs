using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Laborator5_PSSC_DanMirceaAurelian.Domain.Models.ShoppingCarts;
using static LanguageExt.Prelude;
using Laborator5_PSSC_DanMirceaAurelian.Domain.Models;
using Laborator5_PSSC_DanMirceaAurelian.Domain.Repositories;
using Laborator5_PSSC_DanMirceaAurelian.Data.Models;

namespace Laborator5_PSSC_DanMirceaAurelian.Data.Repositories
{
    public class OrderHeadersRepository : IOrderHeadersRepository
    {
        private readonly ShoppingCartsContext shoppingCartsContext;

        public OrderHeadersRepository(ShoppingCartsContext shoppingCartsContext)
        {
            this.shoppingCartsContext = shoppingCartsContext;
        }

        public TryAsync<List<int>> TryGetExistingOrderHeaders(IEnumerable<int> shoppingCartsToCheck) => async () =>
        {
            var orders = await shoppingCartsContext.OrderHeaders
                                                .Where(order => shoppingCartsToCheck.Contains(order.OrderId))
                                                .AsNoTracking()
                                                .ToListAsync();
            return orders.Select(order => order.OrderId)
                            .ToList();
        };
    }
}
