﻿namespace Laborator6_PSSC_DanMirceaAurelian.Domain.Models
{
    public record ValidatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price)
    {   
        public int OrderId { get; set; }
    }


}
