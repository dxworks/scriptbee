using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Plugins;

public class PluginReader(
    ILogger<PluginReader> logger,
    IPluginManifestYamlFileReader pluginManifestYamlFileReader
) : IPluginReader
{
    private const string ManifestYaml = "manifest.yaml";

    public Plugin? ReadPlugin(string pluginFolderPath, PluginId pluginId)
    {
        var pluginPath = Path.Combine(pluginFolderPath, pluginId.GetFullyQualifiedName());
        return ReadPlugin(pluginPath);
    }

    public Plugin? ReadPlugin(string pluginPath)
    {
        logger.LogInformation("Reading manifest from {PluginDirectory}", pluginPath);

        try
        {
            var pluginManifest = ReadManifest(pluginPath);

            if (pluginManifest != null)
            {
                if (TryGetPlugin(pluginPath, pluginManifest, out var plugin))
                {
                    return plugin;
                }

                logger.LogWarning(
                    "Plugin {PluginDirectory} has invalid name or version",
                    pluginPath
                );
                return null;
            }

            logger.LogWarning("Manifest not found in {PluginDirectory}", pluginPath);
            return null;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error reading manifest from {PluginDirectory}", pluginPath);
        }

        return null;
    }

    private static bool TryGetPlugin(
        string pluginPath,
        PluginManifest pluginManifest,
        out Plugin? plugin
    )
    {
        plugin = null;
        var folderName = Path.GetFileName(pluginPath);

        if (PluginId.TryParse(folderName, out var pluginId))
        {
            plugin = new Plugin(pluginPath, pluginId, pluginManifest);
            return true;
        }

        if (pluginManifest.ExtensionPoints.Count <= 0)
        {
            return false;
        }

        var name = $"{pluginManifest.Name}@{pluginManifest.ExtensionPoints[0].Version}";
        if (!PluginId.TryParse(name, out pluginId))
        {
            return false;
        }

        plugin = new Plugin(pluginPath, pluginId, pluginManifest);
        return true;
    }

    public IEnumerable<Plugin> ReadPlugins(string pluginFolderPath)
    {
        if (!Directory.Exists(pluginFolderPath))
        {
            return [];
        }

        var pluginDirectories = Directory.GetDirectories(pluginFolderPath).ToList();

        logger.LogInformation("Found {PluginCount} plugin directories", pluginDirectories.Count);

        return pluginDirectories
            .Select(ReadPlugin)
            .Where(plugin => plugin is not null)
            .OfType<Plugin>();
    }

    private PluginManifest? ReadManifest(string pluginDirectory)
    {
        var path = Path.Combine(pluginDirectory, ManifestYaml);

        return File.Exists(path) ? pluginManifestYamlFileReader.Read(path) : null;
    }
}
