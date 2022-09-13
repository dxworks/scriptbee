using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.FileManagement;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public abstract class DllLoaderPlugin : IPluginLoader
{
    private readonly IFileService _fileService;
    private readonly IDllLoader _dllLoader;

    public abstract string AcceptedPluginKind { get; }

    // todo correlate with plugin types and not to be stored here (like a plugin configuration where the manifest/plugin type is bounded with the
    // todo classes used int assembly scanning)
    protected abstract HashSet<Type> AcceptedPluginTypes { get; }

    protected abstract void RegisterPlugin(PluginManifest pluginManifest, Type @interface, Type concrete);

    protected DllLoaderPlugin(IFileService fileService, IDllLoader dllLoader)
    {
        _fileService = fileService;
        _dllLoader = dllLoader;
    }

    public void Load(Models.Plugin plugin)
    {
        var path = _fileService.CombinePaths(plugin.FolderPath, plugin.Manifest.Metadata.EntryPoint);
        var loadDllTypes = _dllLoader.LoadDllTypes(path, AcceptedPluginTypes);

        foreach (var (@interface, concrete) in loadDllTypes)
        {
            RegisterPlugin(plugin.Manifest, @interface, concrete);
        }
    }
}
