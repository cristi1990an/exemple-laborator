namespace Laborator_6_PSSC.Domain.Models
{
    public record ValidatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price)
    {   
        public int OrderId { get; set; }
    }


}
