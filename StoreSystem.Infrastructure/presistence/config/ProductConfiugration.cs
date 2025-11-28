using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StoreSystem.Infrastructure.Persistence.Config
{
    public class ProductConfiugration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder
            .HasIndex(p => p.SKU)
            .IsUnique();
            
            builder
            .HasIndex(p => p.Barcode)
            .IsUnique();
        }
    }
}