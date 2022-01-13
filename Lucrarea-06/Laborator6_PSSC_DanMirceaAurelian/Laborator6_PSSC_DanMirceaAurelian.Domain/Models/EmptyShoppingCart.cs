namespace Laborator6_PSSC_DanMirceaAurelian.Domain.Models
{
    public record EmptyShoppingCart(string productCode, int quantity, string address, decimal price)
    {
        public int OrderId { get; set; }
    }
}
