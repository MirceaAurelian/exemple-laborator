using Laborator6_PSSC_DanMirceaAurelian.Dto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator6_PSSC_DanMirceaAurelian.Dto.Events
{
    public record ShoppingCartsPublishEvent
    {
        public List<OrderProductDto> ShoppingCarts { get; init; }
    }
}
