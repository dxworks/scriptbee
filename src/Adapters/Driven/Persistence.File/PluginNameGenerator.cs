namespace ScriptBee.Persistence.File;

public static class PluginNameGenerator
{
    public static string GetPluginName(string pluginId, string version)
    {
        return $"{pluginId}@{version}";
    }

    public static string GetPluginName(string pluginId, Version version)
    {
        return GetPluginName(pluginId, version.ToString());
    }

    public static (string? id, Version? version) GetPluginNameAndVersion(string folderName)
    {
        var lastIndexOfDelimiter = folderName.LastIndexOf('@');

        if (lastIndexOfDelimiter == -1)
        {
            return (null, null);
        }

        var id = folderName[..lastIndexOfDelimiter];
        var versionPart = folderName[(lastIndexOfDelimiter + 1)..];

        if (string.IsNullOrWhiteSpace(id))
        {
            return (null, null);
        }

        if (!Version.TryParse(versionPart, out var version))
        {
            return (null, null);
        }

        return (id, version);
    }
}
