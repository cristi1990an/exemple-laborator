using Microsoft.EntityFrameworkCore;
using Laborator4_PSSC.Models;

namespace Laborator4_PSSC
{
    public class ShoppingCartsContext : DbContext
    {
        public ShoppingCartsContext(DbContextOptions<ShoppingCartsContext> options) : base(options)
        {

        }

        public DbSet<OrderHeaderDto> OrderHeaders { get; set; }

        public DbSet<OrderLineDto> OrderLines { get; set; }

        public DbSet<ProductDto> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDto>().ToTable("Product").HasKey(p => p.ProductId);
            modelBuilder.Entity<OrderLineDto>().ToTable("OrderLine").HasKey(ol => ol.OrderLineId);
            modelBuilder.Entity<OrderHeaderDto>().ToTable("OrderHeader").HasKey(oh => oh.OrderId);
        }
    }
}
