using QuickPOS.Models.Common;
using QuickPOS.Models.Enums;

namespace QuickPOS.Models.Entities;

public class Product : BaseAuditableModel
{
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public CategoryModel Category { get; set; } = null!;
    public int MinimumQuantityAlert { get; set; }
    public int ActualQuantity { get; set; }
    public decimal ActualPrice { get; set; }
    public bool IsService { get; set; }
    public string? ImagePath { get; set; }
    public CostingMethod CostingMethod { get; set; } = CostingMethod.Lifo;
    public ICollection<StockBatch> Batches { get; set; } = [];
}
