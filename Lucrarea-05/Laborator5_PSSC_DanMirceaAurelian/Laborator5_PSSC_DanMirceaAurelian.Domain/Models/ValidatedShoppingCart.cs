namespace Laborator5_PSSC_DanMirceaAurelian.Domain.Models
{
    public record ValidatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price);
}
