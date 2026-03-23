using Microsoft.Extensions.Logging;
using ScriptBee.Domain.Model.Plugin.Manifest;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Persistence.File.Plugin;

public class PluginReader(
    ILogger<PluginReader> logger,
    IFileService fileService,
    IPluginManifestYamlFileReader pluginManifestYamlFileReader
) : IPluginReader
{
    private const string ManifestYaml = "manifest.yaml";

    public Domain.Model.Plugin.Plugin? ReadPlugin(string pluginPath)
    {
        logger.LogInformation("Reading manifest from {PluginDirectory}", pluginPath);

        try
        {
            var pluginManifest = ReadManifest(pluginPath);

            if (pluginManifest != null)
            {
                var folderName = fileService.GetFileName(pluginPath);
                var (id, version) = PluginNameGenerator.GetPluginNameAndVersion(folderName);

                if (id is null || version is null)
                {
                    logger.LogWarning(
                        "Plugin {PluginDirectory} has invalid name or version",
                        pluginPath
                    );
                    return null;
                }

                return new Domain.Model.Plugin.Plugin(pluginPath, id, version, pluginManifest);
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

    public IEnumerable<Domain.Model.Plugin.Plugin> ReadPlugins(string pluginFolderPath)
    {
        var pluginDirectories = fileService.GetDirectories(pluginFolderPath).ToList();

        logger.LogInformation("Found {PluginCount} plugin directories", pluginDirectories.Count);

        return pluginDirectories
            .Select(ReadPlugin)
            .Where(plugin => plugin is not null)
            .OfType<Domain.Model.Plugin.Plugin>();
    }

    private PluginManifest? ReadManifest(string pluginDirectory)
    {
        var path = fileService.CombinePaths(pluginDirectory, ManifestYaml);

        return fileService.FileExists(path) ? pluginManifestYamlFileReader.Read(path) : null;
    }
}
