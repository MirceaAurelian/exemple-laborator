namespace Laborator2_PSSC_DanMirceaAurelian.Domain
{
    public record UnvalidatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price);
}
