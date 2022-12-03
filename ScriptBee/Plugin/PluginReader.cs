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

    public void ClearDeletePluginsFolder(string pluginFolderPath)
    {
        var deleteFolders = _fileService.CombinePaths(pluginFolderPath, "delete.txt");

        if (!_fileService.FileExists(deleteFolders))
        {
            return;
        }

        var deleteFoldersContent = _fileService.ReadAllText(deleteFolders);

        var foldersToDelete = deleteFoldersContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        foreach (var folderToDelete in foldersToDelete)
        {
            var folderToDeletePath = _fileService.CombinePaths(pluginFolderPath, folderToDelete);
            _fileService.DeleteFolder(folderToDeletePath);
        }

        _fileService.DeleteFile(deleteFolders);
    }

    public IEnumerable<Models.Plugin> ReadPlugins(string pluginFolderPath)
    {
        var plugins = new List<Models.Plugin>();

        var pluginDirectories = _fileService.GetDirectories(pluginFolderPath).ToList();

        _logger.Information("Found {PluginCount} plugin directories", pluginDirectories.Count);

        foreach (var pluginDirectory in pluginDirectories)
        {
            _logger.Information("Reading manifest from {PluginDirectory}", pluginDirectory);

            try
            {
                var pluginManifest = ReadManifest(pluginDirectory);

                if (pluginManifest != null)
                {
                    var folderName = _fileService.GetFileName(pluginDirectory);
                    var (id, version) = PluginNameGenerator.GetPluginNameAndVersion(folderName);

                    if (id is null || version is null)
                    {
                        _logger.Warning("Plugin {PluginDirectory} has invalid name or version", pluginDirectory);
                        continue;
                    }

                    plugins.Add(new Models.Plugin(pluginDirectory, id, version, pluginManifest));
                }
                else
                {
                    _logger.Warning("Manifest not found in {PluginDirectory}", pluginDirectory);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error reading manifest from {PluginDirectory}", pluginDirectory);
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
