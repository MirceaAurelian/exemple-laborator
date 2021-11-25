namespace Laborator4_PSSC_DanMirceaAurelian.Domain
{
    public record ValidatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price);
}
