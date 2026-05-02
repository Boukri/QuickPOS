using System.ComponentModel.DataAnnotations.Schema;

namespace QuickPOS.Models.Common;
public abstract class EntityStatus
{
    public string Status { get; set; } = default!;
    protected EntityStatus(string status)
    {
        if (IsValidStatus(status) && string.IsNullOrEmpty(status)) Status = status;
    }
    [NotMapped]
    public static string Active { get; set; } = "Active";
    [NotMapped]
    public static string Inactive { get; set; } = "Inactive";

    public static bool IsValidStatus(string status)
    {
        return status == Active || status == Inactive;
    }
}
