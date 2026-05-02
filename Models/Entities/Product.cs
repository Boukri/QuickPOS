using QuickPOS.Models.Common;
using QuickPOS.Models.Enums;

namespace QuickPOS.Models.Entities;

public class Product : BaseAuditableModel
{
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public CategoryModel Category { get; set; } = null!;
    public string Sku { get; set; } = string.Empty;
    public decimal WholesalePrice { get; set; }
    public decimal RetailPrice { get; set; }
    public bool IsService { get; set; }
    public string? ImagePath { get; set; }
    public CostingMethod CostingMethod { get; set; } = CostingMethod.Fifo;
    public ICollection<StockBatch> Batches { get; set; } = [];
}
