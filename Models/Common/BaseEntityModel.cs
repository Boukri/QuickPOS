using QuickPOS.Models.Abstraction;

namespace QuickPOS.Models.Common;
public abstract class BaseEntityModel : IBaseEntity
{
    public int Id { get; set; }
}
