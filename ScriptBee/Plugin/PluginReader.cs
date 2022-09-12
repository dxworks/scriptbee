﻿using System;
using System.Collections.Generic;
using System.Linq;
using ScriptBee.FileManagement;
using ScriptBee.Plugin.Manifest;
using Serilog;

namespace ScriptBee.Plugin;

public class PluginReader : IPluginReader
{
    private const string ManifestYaml = "manifest.yaml";

    private readonly ILogger _logger;
    private readonly IFileService _fileService;
    private readonly IPluginManifestYamlFileReader _pluginManifestYamlFileReader;
    private readonly IPluginManifestValidator _manifestValidator;

    public PluginReader(ILogger logger, IFileService fileService,
        IPluginManifestYamlFileReader pluginManifestYamlFileReader,
        IPluginManifestValidator manifestValidator)
    {
        _logger = logger;
        _fileService = fileService;
        _pluginManifestYamlFileReader = pluginManifestYamlFileReader;
        _manifestValidator = manifestValidator;
    }

    public IEnumerable<Models.Plugin> ReadPlugins(string pluginFolderPath)
    {
        var plugins = new List<Models.Plugin>();

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
                        plugins.Add(new Models.Plugin(pluginDirectory, pluginManifest));
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

        return plugins;
    }

    private PluginManifest? ReadManifest(string pluginDirectory)
    {
        var path = _fileService.CombinePaths(pluginDirectory, ManifestYaml);

        return _fileService.FileExists(path) ? _pluginManifestYamlFileReader.Read(path) : null;
    }
}
