using System;
using System.Collections.Generic;
using ScriptBee.FileManagement;

namespace ScriptBee.Plugin;

public abstract class DllLoaderPlugin : IPluginLoader
{
    private readonly IFileService _fileService;
    private readonly IDllLoader _dllLoader;
    private readonly IPluginRepository _pluginRepository;
    private readonly IPluginService _pluginService;

    public abstract string AcceptedPluginKind { get; }

    // todo correlate with plugin types and not to be stored here (like a plugin configuration where the manifest/plugin type is bounded with the
    // todo classes used int assembly scanning)
    protected abstract HashSet<Type> AcceptedPluginTypes { get; }

    protected DllLoaderPlugin(IFileService fileService, IDllLoader dllLoader, IPluginRepository pluginRepository,
        IPluginService pluginService)
    {
        _fileService = fileService;
        _dllLoader = dllLoader;
        _pluginRepository = pluginRepository;
        _pluginService = pluginService;
    }

    public void Load(Models.Plugin plugin)
    {
        _pluginService.Add(plugin.Manifest);

        var path = _fileService.CombinePaths(plugin.FolderPath, plugin.Manifest.Metadata.EntryPoint);
        var loadDllTypes = _dllLoader.LoadDllTypes(path, AcceptedPluginTypes);

        foreach (var type in loadDllTypes)
        {
            // todo add ServiceCollection to use the di container
            var pluginInstance = Activator.CreateInstance(type);
            if (pluginInstance != null)
            {
                _pluginRepository.RegisterPlugin(pluginInstance);
            }
        }
    }
}
