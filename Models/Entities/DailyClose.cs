using QuickPOS.Models.Common;

namespace QuickPOS.Models.Entities;

public class DailyClose : BaseAuditableModel
{
    public decimal SystemCashTotal { get; set; }
    public decimal SystemCardTotal { get; set; }
    public decimal SystemPointsTotal { get; set; }
    public decimal ManualCashTotal { get; set; }
    public decimal CashDiscrepancy => ManualCashTotal - SystemCashTotal;
    public string ClosedBy { get; set; } = string.Empty;
}
