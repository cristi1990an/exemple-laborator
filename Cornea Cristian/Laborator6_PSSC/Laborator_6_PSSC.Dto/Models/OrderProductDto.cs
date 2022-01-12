

namespace Laborator_6_PSSC.Dto.Models
{
    public record OrderProductDto
    {
        public string Name { get; init; }
        public string Address { get; init; }
        public string ProductCode { get; init; }
        public decimal Price { get; init; }
        public int Quantity { get; init; }
        public decimal FinalPrice { get; init; }
    }
}
