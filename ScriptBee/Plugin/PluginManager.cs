using System;
using System.Linq;
using System.Reflection;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Plugin.Manifest;
using Serilog;

namespace ScriptBee.Plugin;

public class PluginManager
{
    private readonly ILogger _logger;
    private readonly IPluginManifestReader _pluginManifestReader;
    private readonly IPluginLoaderFactory _pluginLoaderFactory;
    private readonly IPluginRepository _pluginRepository;

    public PluginManager(ILogger logger, IPluginManifestReader pluginManifestReader,
        IPluginLoaderFactory pluginLoaderFactory, IPluginRepository pluginRepository)
    {
        _pluginManifestReader = pluginManifestReader;
        _pluginLoaderFactory = pluginLoaderFactory;
        _pluginRepository = pluginRepository;
        _logger = logger;
    }

    public void LoadPlugins(string pluginFolder)
    {
        // todo filter custom plugin definitions first
        // todo iterate over all plugin definitions and load them
        var pluginManifests = _pluginManifestReader.ReadManifests(pluginFolder);

        foreach (var pluginManifest in pluginManifests)
        {
            switch (pluginManifest)
            {
                case ScriptGeneratorPluginManifest:
                    var services = new ServiceCollection()
                        .AddSingleton<IFileContentProvider, RelativeFileContentProvider>();
                    LoadDllPlugin<IScriptGeneratorStrategy>(pluginManifest.Metadata.EntryPoint, services);
                    // _pluginRepository.RegisterPlugin<IScriptGeneratorStrategy>(scriptGeneratorSpec);
                    break;
                // case UiPluginManifestSpec uiPluginManifestSpec:
                //     // todo register ui plugin
                //     break;
                default:
                    _logger.Warning("Unknown plugin type {plugin}", pluginManifest);
                    break;
            }
        }
    }

    private void LoadDllPlugin<T>(string pathToDll, IServiceCollection services) where T : class
    {
        Assembly CurrentDomainOnAssemblyResolve(object? sender, ResolveEventArgs args)
        {
            return ((AppDomain)sender).GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name);
        }

        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

        var pluginDll = Assembly.LoadFrom(pathToDll);

        // todo take into consideration the plugin manifest type to support more than just dlls
        foreach (var type in pluginDll.GetExportedTypes())
        {
            var pluginLoader = _pluginLoaderFactory.GetPluginLoader(type);
            if (pluginLoader is null)
            {
                _logger.Warning("Plugin loader not found for type {type}", type);
            }
            else
            {
                if (Activator.CreateInstance(type) is T t)
                {
                    services.AddSingleton(t);
                }
                // pluginLoader.LoadPlugin(pluginManifest, type);
            }
        }

        AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
    }
}
