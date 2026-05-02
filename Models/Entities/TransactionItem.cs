

using QuickPOS.Models.Common;

namespace QuickPOS.Models.Entities;

public class TransactionItem : BaseAuditableModel
{ 
    public int TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    /// <summary>Cost of Goods Sold recorded at the time of sale (per line item).</summary>
    public decimal Cogs { get; set; }
}
