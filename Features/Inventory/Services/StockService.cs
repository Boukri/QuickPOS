using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuickPOS.Data;
using QuickPOS.Models.Entities;
using QuickPOS.Models.Enums;

namespace QuickPOS.Features.Inventory;

/// <summary>
/// Implements FIFO / LIFO / WAC depletion with:
///  - Atomic multi-batch writes (single DB transaction per sale).
///  - Optimistic concurrency via QuantityRemaining token: retries on conflict.
/// </summary>
public sealed class StockService(IDbContextFactory<QuickPosDbContext> factory) : IStockService
{
    private const int MaxRetries = 3;

    /// <inheritdoc/>
    public async Task<SaleResult?> ProcessSaleAsync(
        int productId,
        int quantityRequested,
        CostingMethod costingMethod = CostingMethod.Fifo,
        CancellationToken ct = default)
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            await using var ctx = await factory.CreateDbContextAsync(ct);
            await using var tx = await ctx.Database.BeginTransactionAsync(ct);

            try
            {
                // Load active batches; order depends on method.
                IQueryable<StockBatch> query = ctx.StockBatches
                    .Where(b => b.ProductId == productId && b.QuantityRemaining > 0);

                query = costingMethod == CostingMethod.Lifo
                    ? query.OrderByDescending(b => b.ReceivedAt)
                    : query.OrderBy(b => b.ReceivedAt); // FIFO and WAC both read oldest-first

                var batches = await query.ToListAsync(ct);

                int available = batches.Sum(b => b.QuantityRemaining);
                if (available < quantityRequested)
                    return null;

                decimal cogs;

                if (costingMethod == CostingMethod.WeightedAverage)
                {
                    // WAC: all units cost the same regardless of which batch they came from.
                    decimal totalValue = batches.Sum(b => (decimal)b.QuantityRemaining * b.UnitCost);
                    decimal wac = Math.Round(totalValue / available, 4);
                    cogs = quantityRequested * wac;

                    // Deplete batches FIFO-style (order doesn't affect COGS under WAC).
                    int remaining = quantityRequested;
                    foreach (var batch in batches)
                    {
                        if (remaining <= 0) break;
                        int taken = Math.Min(remaining, batch.QuantityRemaining);
                        batch.QuantityRemaining -= taken;
                        remaining -= taken;
                    }
                }
                else
                {
                    // FIFO or LIFO: COGS = sum of (units taken × historical unit cost).
                    cogs = 0m;
                    int remaining = quantityRequested;

                    foreach (var batch in batches)
                    {
                        if (remaining <= 0) break;
                        int taken = Math.Min(remaining, batch.QuantityRemaining);
                        cogs += taken * batch.UnitCost;
                        batch.QuantityRemaining -= taken;
                        remaining -= taken;
                    }
                }

                await ctx.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                return new SaleResult(cogs, quantityRequested);
            }
            catch (DbUpdateConcurrencyException) when (attempt < MaxRetries - 1)
            {
                await tx.RollbackAsync(ct);
                await Task.Delay(TimeSpan.FromMilliseconds(50 * (attempt + 1)), ct);
            }
        }

        throw new InvalidOperationException(
            $"Sale for product {productId} failed after {MaxRetries} retries due to concurrent modifications.");
    }

    /// <inheritdoc/>
    /// <inheritdoc/>
    public async Task SaveBatchDepletionsAsync(
        IReadOnlyList<(int BatchId, int NewQuantityRemaining)> depletions,
        CancellationToken ct = default)
    {
        if (depletions.Count == 0) return;

        var ids = depletions.Select(d => d.BatchId).ToList();

        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            await using var ctx = await factory.CreateDbContextAsync(ct);
            await using var tx = await ctx.Database.BeginTransactionAsync(ct);

            try
            {
                // Load only the rows that need updating — one query for all batches.
                var batches = await ctx.StockBatches
                    .Where(b => ids.Contains(b.Id))
                    .ToListAsync(ct);

                // Apply pre-computed values; QuantityRemaining is the concurrency token.
                foreach (var (batchId, newQty) in depletions)
                {
                    var batch = batches.FirstOrDefault(b => b.Id == batchId);
                    if (batch is not null)
                        batch.QuantityRemaining = Math.Max(0, newQty);
                }

                await ctx.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return;
            }
            catch (DbUpdateConcurrencyException) when (attempt < MaxRetries - 1)
            {
                await tx.RollbackAsync(ct);
                await Task.Delay(TimeSpan.FromMilliseconds(50 * (attempt + 1)), ct);
            }
        }

        throw new InvalidOperationException(
            $"SaveBatchDepletions failed after {MaxRetries} retries due to concurrent modifications.");
    }

    /// <inheritdoc/>
    public async Task ReceiveStockAsync(
        int productId,
        int quantityReceived,
        decimal unitCost,
        DateTime receivedAt,
        string? reference = null,
        CancellationToken ct = default)
    {
        await using var ctx = await factory.CreateDbContextAsync(ct);

        ctx.StockBatches.Add(new StockBatch
        {
            ProductId       = productId,
            QuantityReceived  = quantityReceived,
            QuantityRemaining = quantityReceived,
            UnitCost        = unitCost,
            ReceivedAt      = receivedAt,
            Reference       = reference
        });

        await ctx.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<decimal> GetWeightedAverageCostAsync(
        int productId,
        CancellationToken ct = default)
    {
        await using var ctx = await factory.CreateDbContextAsync(ct);

        // Project only what we need — avoids loading full entities.
        var batches = await ctx.StockBatches
            .Where(b => b.ProductId == productId && b.QuantityRemaining > 0)
            .Select(b => new { b.QuantityRemaining, b.UnitCost })
            .ToListAsync(ct);

        int totalUnits = batches.Sum(b => b.QuantityRemaining);
        if (totalUnits == 0) return 0m;

        // WAC = total remaining inventory value ÷ total remaining units.
        // This is recalculated live from batch data — no denormalized column needed.
        decimal totalValue = batches.Sum(b => b.QuantityRemaining * b.UnitCost);
        return Math.Round(totalValue / totalUnits, 4);
    }

    /// <inheritdoc/>
    public async Task<int> GetTotalUnitsAsync(int productId, CancellationToken ct = default)
    {
        await using var ctx = await factory.CreateDbContextAsync(ct);
        return await ctx.StockBatches
            .Where(b => b.ProductId == productId && b.QuantityRemaining > 0)
            .SumAsync(b => b.QuantityRemaining, ct);
    }
}
