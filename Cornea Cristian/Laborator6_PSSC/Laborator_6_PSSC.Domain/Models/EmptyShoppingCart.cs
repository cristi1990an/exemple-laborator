
namespace Laborator_6_PSSC.Domain.Models
{
    public record EmptyShoppingCart(string productCode, int quantity, string address, decimal price)
    {
        public int OrderId { get; set; }
    }
}
