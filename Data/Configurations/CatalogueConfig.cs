using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickPOS.Models.Entities;
using QuickPOS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickPOS.Data.Configurations;
internal class CategoryConfig : IEntityTypeConfiguration<CategoryModel>
{
    public void Configure(EntityTypeBuilder<CategoryModel> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasMany(c => c.Products)
               .WithOne(p => p.Category)
               .HasForeignKey(p => p.CategoryId);
    }
}

internal class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.WholesalePrice).HasColumnType("decimal(18,2)");
        builder.Property(p => p.RetailPrice).HasColumnType("decimal(18,2)");
        builder.Property(p => p.CostingMethod)
               .HasConversion<string>()
               .HasMaxLength(20)
               .HasDefaultValue(CostingMethod.Fifo);
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasMany(p => p.Batches)
               .WithOne(b => b.Product)
               .HasForeignKey(b => b.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class StockBatchConfig : IEntityTypeConfiguration<StockBatch>
{
    public void Configure(EntityTypeBuilder<StockBatch> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.UnitCost).HasColumnType("decimal(18,2)");
        builder.Property(b => b.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
        builder.HasIndex(b => new { b.ProductId, b.ReceivedAt });

        // Optimistic concurrency: EF appends "AND QuantityRemaining = @original" to every UPDATE.
        // If another session depleted this batch between our SELECT and UPDATE,
        // the affected row count is 0 → EF throws DbUpdateConcurrencyException
        // → StockService retries up to MaxRetries times with a fresh read.
        builder.Property(b => b.QuantityRemaining).IsConcurrencyToken();
    }
}
