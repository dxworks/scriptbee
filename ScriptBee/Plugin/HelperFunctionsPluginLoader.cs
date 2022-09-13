using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.FileManagement;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public class HelperFunctionsPluginLoader : DllLoaderPlugin
{
    private readonly IPluginRepository _pluginRepository;
    public override string AcceptedPluginKind => PluginTypes.HelperFunctions;

    protected override HashSet<Type> AcceptedPluginTypes { get; } = new()
    {
        typeof(IHelperFunctions)
    };

    public HelperFunctionsPluginLoader(IFileService fileService, IDllLoader dllLoader, IPluginRepository pluginRepository)
        : base(fileService, dllLoader)
    {
        _pluginRepository = pluginRepository;
    }

    protected override void RegisterPlugin(PluginManifest pluginManifest, Type @interface, Type concrete)
    {
        _pluginRepository.RegisterPlugin(pluginManifest, @interface, concrete);
    }
}
