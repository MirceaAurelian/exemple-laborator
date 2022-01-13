using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator6_PSSC_DanMirceaAurelian.Dto.Models
{
    public record OrderProductDto
    {
        public string Name { get; init; }
        public string Address { get; init; }
        public string ProductCode { get; init; }
        public int OrderId { get; init; }
        public int OrderLineId { get; init; }
        public decimal Price { get; init; }
        public int Quantity { get; init; }
        public decimal FinalPrice { get; init; }
    }
}
