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
                var folderName = Path.GetFileName(pluginPath);
                if (PluginId.TryParse(folderName, out var pluginId))
                {
                    return new Plugin(pluginPath, pluginId, pluginManifest);
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
