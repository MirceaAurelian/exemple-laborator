namespace Laborator6_PSSC_DanMirceaAurelian.Domain.Models
{
    public record UnvalidatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price);
}
