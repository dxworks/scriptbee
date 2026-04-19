using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Plugins;

public class PluginReader(
    ILogger<PluginReader> logger,
    IPluginManifestYamlFileReader pluginManifestYamlFileReader
) : IPluginReader
{
    private const string ManifestYaml = "manifest.yaml";

    public Plugin? ReadPlugin(string pluginFolderPath, string pluginId, string version)
    {
        var pluginName = PluginNameGenerator.GetPluginName(pluginId, version);
        var pluginPath = Path.Combine(pluginFolderPath, pluginName);
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
                var (id, version) = PluginNameGenerator.GetPluginNameAndVersion(folderName);

                if (id is null || version is null)
                {
                    logger.LogWarning(
                        "Plugin {PluginDirectory} has invalid name or version",
                        pluginPath
                    );
                    return null;
                }

                return new Plugin(pluginPath, id, version, pluginManifest);
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
