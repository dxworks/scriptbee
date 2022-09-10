using System;
using System.Collections.Generic;
using System.Linq;
using ScriptBee.FileManagement;
using Serilog;

namespace ScriptBee.Plugin;

public class PluginManifestReader : IPluginManifestReader
{
    private const string ManifestYaml = "manifest.yaml";

    private readonly ILogger _logger;
    private readonly IFileService _fileService;
    private readonly IYamlFileReader _yamlFileReader;
    private readonly IPluginManifestValidator _manifestValidator;

    public PluginManifestReader(ILogger logger, IFileService fileService, IYamlFileReader yamlFileReader,
        IPluginManifestValidator manifestValidator)
    {
        _logger = logger;
        _fileService = fileService;
        _yamlFileReader = yamlFileReader;
        _manifestValidator = manifestValidator;
    }

    public IEnumerable<PluginManifest> ReadManifests(string pluginFolderPath)
    {
        var pluginManifests = new List<PluginManifest>();

        var pluginDirectories = _fileService.GetDirectories(pluginFolderPath).ToList();

        _logger.Information("Found {pluginCount} plugin directories", pluginDirectories.Count);

        foreach (var pluginDirectory in pluginDirectories)
        {
            _logger.Information("Reading manifest from {pluginDirectory}", pluginDirectory);

            try
            {
                var pluginManifest = ReadManifest(pluginDirectory);

                if (pluginManifest != null)
                {
                    if (_manifestValidator.Validate(pluginManifest))
                    {
                        pluginManifests.Add(pluginManifest);
                    }
                    else
                    {
                        _logger.Warning("Manifest validation failed for {pluginDirectory}", pluginDirectory);
                    }
                }
                else
                {
                    _logger.Warning("Manifest not found in {pluginDirectory}", pluginDirectory);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error reading manifest from {pluginDirectory}", pluginDirectory);
            }
        }

        return pluginManifests;
    }

    private PluginManifest? ReadManifest(string pluginDirectory)
    {
        var path = _fileService.CombinePaths(pluginDirectory, ManifestYaml);

        return _fileService.FileExists(path) ? _yamlFileReader.Read<PluginManifest>(path) : null;
    }
}
