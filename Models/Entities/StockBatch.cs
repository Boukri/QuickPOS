using QuickPOS.Models.Common;

namespace QuickPOS.Models.Entities;

/// <summary>
/// Represents a single purchase receipt for a product (one "layer" in FIFO/WAC tracking).
/// </summary>
public class StockBatch : BaseAuditableModel
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Units received in this batch.</summary>
    public int QuantityReceived { get; set; }

    /// <summary>Units still available (decremented by FIFO sales).</summary>
    public int QuantityRemaining { get; set; }

    /// <summary>Purchase cost per unit for this batch.</summary>
    public decimal UnitCost { get; set; }

    /// <summary>UTC date/time this batch was received (determines FIFO order).</summary>
    public DateTime ReceivedAt { get; set; }

    /// <summary>Optional reference number (PO, invoice, etc.).</summary>
    public string? Reference { get; set; }
}
