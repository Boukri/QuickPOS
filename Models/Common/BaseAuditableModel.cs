using QuickPOS.Models.Abstraction;

using QuickPOS.Models.Abstraction;

namespace QuickPOS.Models.Common;
public abstract class BaseAuditableModel : BaseEntityModel, IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
