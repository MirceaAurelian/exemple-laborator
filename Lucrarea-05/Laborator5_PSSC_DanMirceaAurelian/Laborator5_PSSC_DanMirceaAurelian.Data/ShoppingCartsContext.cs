﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Laborator5_PSSC_DanMirceaAurelian.Data.Models;

namespace Laborator5_PSSC_DanMirceaAurelian.Data
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
