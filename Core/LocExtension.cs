using System.Windows.Markup;

namespace QuickPOS.Core;

/// <summary>
/// Markup extension that binds a property on the singleton LocalizationService.
/// Usage: Text="{loc:Loc NavPosTerminal}"
/// </summary>
[MarkupExtensionReturnType(typeof(object))]
public class LocExtension : MarkupExtension
{
    public string Key { get; set; } = string.Empty;

    public LocExtension() { }
    public LocExtension(string key) { Key = key; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new System.Windows.Data.Binding(Key)
        {
            Source = LocalizationService.Instance,
            Mode = System.Windows.Data.BindingMode.OneWay
        };
        return binding.ProvideValue(serviceProvider);
    }
}
