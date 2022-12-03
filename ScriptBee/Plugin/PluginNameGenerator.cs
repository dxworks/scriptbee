using System;

namespace ScriptBee.Plugin;

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
        var parts = folderName.Split('@');

        if (parts.Length == 2)
        {
            return (parts[0], Version.Parse(parts[1]));
        }

        return (null, null);
    }
}
