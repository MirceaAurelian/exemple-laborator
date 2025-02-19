﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Laborator4_PSSC_DanMirceaAurelian.Domain.Repositories;
using static Laborator4_PSSC_DanMirceaAurelian.Domain.ShoppingCarts;
using static LanguageExt.Prelude;
using Laborator4_PSSC_DanMirceaAurelian.Domain;
using Laborator4_PSSC_DanMirceaAurelian.Models;

namespace Laborator4_PSSC_DanMirceaAurelian.Repositories
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
