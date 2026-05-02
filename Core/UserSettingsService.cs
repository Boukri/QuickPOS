using System.IO;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace QuickPOS.Core;

[XmlRoot("QuickPOSSettings")]
public class UserSettings
{
    [XmlElement] public double CartPanelWidth { get; set; } = 400;
    [XmlElement] public AppLanguage Language { get; set; } = AppLanguage.French;
    [XmlElement] public bool RememberMe { get; set; } = false;
    [XmlElement] public string AutoLoginUsername { get; set; } = string.Empty;
}

public class UserSettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "QuickPOS", "usersettings.config");

    private static readonly XmlSerializer Serializer = new(typeof(UserSettings));

    private UserSettings _settings = new();

    public UserSettings Settings => _settings;

    public void Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                using var reader = XmlReader.Create(SettingsPath);
                _settings = (UserSettings?)Serializer.Deserialize(reader) ?? new();
            }
        }
        catch
        {
            _settings = new();
        }
    }

    public void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var writerSettings = new XmlWriterSettings { Indent = true };
            using var writer = XmlWriter.Create(SettingsPath, writerSettings);
            Serializer.Serialize(writer, _settings);
        }
        catch
        {
            // Silently ignore write failures
        }
    }
}
