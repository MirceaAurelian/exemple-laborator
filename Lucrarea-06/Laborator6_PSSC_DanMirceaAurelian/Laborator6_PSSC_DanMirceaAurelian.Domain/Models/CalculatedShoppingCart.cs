using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator6_PSSC_DanMirceaAurelian.Domain.Models
{
    public record CalculatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price, Price finalPrice)
    {
        public int OrderId { get; set; }
        public int OrderLineId { get; set; }
        public int IsUpdated { get; set; }
    }
}
