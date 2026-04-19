using System.Diagnostics.CodeAnalysis;

namespace ScriptBee.Domain.Model.Plugins;

public record PluginId(string Name, Version Version)
{
    public string GetFullyQualifiedName()
    {
        return $"{Name}@{Version}";
    }

    public static bool TryParse(string folderName, [NotNullWhen(true)] out PluginId? pluginInfo)
    {
        pluginInfo = null;
        var lastIndexOfDelimiter = folderName.LastIndexOf('@');

        if (lastIndexOfDelimiter == -1)
        {
            return false;
        }

        var id = folderName[..lastIndexOfDelimiter];
        var versionPart = folderName[(lastIndexOfDelimiter + 1)..];

        if (string.IsNullOrWhiteSpace(id) || !Version.TryParse(versionPart, out var version))
        {
            return false;
        }

        pluginInfo = new PluginId(id, version);
        return true;
    }
}
