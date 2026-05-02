using QuickPOS.Models.Common;

namespace QuickPOS.Models.Entities;

public class Transaction : BaseAuditableModel
{ 
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string Status { get; set; } = "Completed";
    public List<TransactionItem> Items { get; set; } = [];
}