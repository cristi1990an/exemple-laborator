
namespace Laborator4_PSSC.Domain
{
    public record CalculatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price, Price finalPrice)
    {
        public int OrderId { get; set; }
        public bool IsUpdated { get; set; }
    }
}
