
namespace Laborator_6_PSSC.Domain.Models
{
    public record CalculatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price, Price finalPrice)
    {
        public int OrderId { get; set; }
        public int OrderLineId { get; set; }
        public int IsUpdated { get; set; }
    }
}
