using System;
using System.Collections.Generic;
using System.Linq;
using ScriptBee.FileManagement;
using ScriptBee.Plugin.Manifest;
using Serilog;

namespace ScriptBee.Plugin;

public class PluginReader : IPluginReader
{
    private const string ManifestYaml = "manifest.yaml";
    private readonly IFileService _fileService;

    private readonly ILogger _logger;
    private readonly IPluginManifestYamlFileReader _pluginManifestYamlFileReader;

    public PluginReader(ILogger logger, IFileService fileService,
        IPluginManifestYamlFileReader pluginManifestYamlFileReader)
    {
        _logger = logger;
        _fileService = fileService;
        _pluginManifestYamlFileReader = pluginManifestYamlFileReader;
    }

    public Models.Plugin? ReadPlugin(string pluginPath)
    {
        _logger.Information("Reading manifest from {PluginDirectory}", pluginPath);

        try
        {
            var pluginManifest = ReadManifest(pluginPath);

            if (pluginManifest != null)
            {
                var folderName = _fileService.GetFileName(pluginPath);
                var (id, version) = PluginNameGenerator.GetPluginNameAndVersion(folderName);

                if (id is null || version is null)
                {
                    _logger.Warning("Plugin {PluginDirectory} has invalid name or version", pluginPath);
                    return null;
                }

                return new Models.Plugin(pluginPath, id, version, pluginManifest);
            }

            _logger.Warning("Manifest not found in {PluginDirectory}", pluginPath);
            return null;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error reading manifest from {PluginDirectory}", pluginPath);
        }

        return null;
    }

    public IEnumerable<Models.Plugin> ReadPlugins(string pluginFolderPath)
    {
        var pluginDirectories = _fileService.GetDirectories(pluginFolderPath).ToList();

        _logger.Information("Found {PluginCount} plugin directories", pluginDirectories.Count);

        return pluginDirectories.Select(ReadPlugin)
            .Where(plugin => plugin is not null)
            .OfType<Models.Plugin>();
    }

    private PluginManifest? ReadManifest(string pluginDirectory)
    {
        var path = _fileService.CombinePaths(pluginDirectory, ManifestYaml);

        return _fileService.FileExists(path) ? _pluginManifestYamlFileReader.Read(path) : null;
    }
}
