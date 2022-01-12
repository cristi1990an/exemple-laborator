namespace Laborator_6_PSSC.Domain.Models
{
    public record UnvalidatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price);
}
