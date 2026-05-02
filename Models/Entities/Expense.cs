using QuickPOS.Models.Common;

namespace QuickPOS.Models.Entities;

public class Expense : BaseAuditableModel
{ 
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty; 
}
