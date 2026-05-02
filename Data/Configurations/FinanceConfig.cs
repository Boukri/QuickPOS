using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickPOS.Models.Entities;

namespace QuickPOS.Data.Configurations;
internal class DailyCloseConfig : IEntityTypeConfiguration<DailyClose>
{
    public void Configure(EntityTypeBuilder<DailyClose> builder)
    {
        builder.HasKey(dc => dc.Id);
        builder.Property(dc => dc.SystemCashTotal).HasColumnType("decimal(18,2)");
        builder.Property(dc => dc.SystemCardTotal).HasColumnType("decimal(18,2)");
        builder.Property(dc => dc.SystemPointsTotal).HasColumnType("decimal(18,2)");
        builder.Property(dc => dc.ManualCashTotal).HasColumnType("decimal(18,2)");
        builder.Property(dc => dc.ClosedBy).IsRequired().HasMaxLength(100);
    }
}
internal class ExpenseConfig : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(ex => ex.Id);
        builder.Property(ex => ex.Amount).HasColumnType("decimal(18,2)");
    }
}

internal class TransactionConfig : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TotalAmount).HasColumnType("decimal(18,2)");
        builder.HasMany(t => t.Items).WithOne(i => i.Transaction).HasForeignKey(i => i.TransactionId);
    }
}

internal class TransactionItemConfig : IEntityTypeConfiguration<TransactionItem>
{
    public void Configure(EntityTypeBuilder<TransactionItem> builder)
    {
        builder.HasKey(ti => ti.Id);
        builder.Property(ti => ti.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(ti => ti.Cogs).HasColumnType("decimal(18,4)").HasDefaultValue(0m);
    }
}