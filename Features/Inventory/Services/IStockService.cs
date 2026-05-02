using QuickPOS.Models.Enums;
using QuickPOS.Models.Enums;

namespace QuickPOS.Features.Inventory;

/// <summary>Result of a stock sale operation.</summary>
/// <param name="Cogs">Cost of Goods Sold for the depleted units.</param>
/// <param name="UnitsProcessed">Units actually depleted.</param>
public sealed record SaleResult(decimal Cogs, int UnitsProcessed);

public interface IStockService
{
    /// <summary>
    /// Depletes stock for a single product using the specified costing method.
    /// Returns <c>null</c> when available stock is insufficient.
    /// </summary>
    Task<SaleResult?> ProcessSaleAsync(
        int productId,
        int quantityRequested,
        CostingMethod costingMethod = CostingMethod.Fifo,
        CancellationToken ct = default);

    /// <summary>
    /// Persists pre-computed batch depletions in a single atomic transaction.
    /// The caller computes <c>NewQuantityRemaining</c> from data already in memory.
    /// Each batch is loaded by ID, the new quantity is applied, and all rows are
    /// written in one <c>SaveChangesAsync</c> call with concurrency-token protection.
    /// </summary>
    Task SaveBatchDepletionsAsync(
        IReadOnlyList<(int BatchId, int NewQuantityRemaining)> depletions,
        CancellationToken ct = default);

    /// <summary>Persists a new stock receipt batch for a product.</summary>
    Task ReceiveStockAsync(
        int productId,
        int quantityReceived,
        decimal unitCost,
        DateTime receivedAt,
        string? reference = null,
        CancellationToken ct = default);

    /// <summary>WAC across active batches; returns 0 when no stock exists.</summary>
    Task<decimal> GetWeightedAverageCostAsync(int productId, CancellationToken ct = default);

    /// <summary>Total available units across active batches.</summary>
    Task<int> GetTotalUnitsAsync(int productId, CancellationToken ct = default);
}
