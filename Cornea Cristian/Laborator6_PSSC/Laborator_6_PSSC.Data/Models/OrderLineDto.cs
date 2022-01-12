
namespace Laborator_6_PSSC.Data.Models
{
    public class OrderLineDto
    {
        public int OrderLineId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
