using QuickPOS.Models.Common;

namespace QuickPOS.Models.Entities;
public class CategoryModel : BaseAuditableModel
{
    public string Name { get; set; } = string.Empty;
    public bool IsService { get; set; }
    public List<Product> Products { get; set; } = [];
}
