namespace QuickPOS.Models.Enums;

/// <summary>
/// Inventory costing method used to calculate COGS when a sale is processed.
/// </summary>
public enum CostingMethod
{
    /// <summary>First-In, First-Out: oldest batches are consumed first.</summary>
    Fifo = 0,

    /// <summary>Last-In, First-Out: newest batches are consumed first.</summary>
    Lifo = 1,

    /// <summary>Weighted Average Cost: COGS = units sold × (total value / total units).</summary>
    WeightedAverage = 2
}
