using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Laborator4_PSSC_DanMirceaAurelian.Models;

namespace Laborator4_PSSC_DanMirceaAurelian
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
            modelBuilder.Entity<OrderLineDto>().ToTable("OrderLineId").HasKey(ol => ol.OrderLineId);
            modelBuilder.Entity<OrderHeaderDto>().ToTable("OrderId").HasKey(oh => oh.OrderId);
        }
    }
}
