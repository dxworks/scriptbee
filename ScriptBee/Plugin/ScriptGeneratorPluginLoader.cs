using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.FileManagement;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Plugin;

public class ScriptGeneratorPluginLoader : DllLoaderPlugin
{
    public override string AcceptedPluginKind => PluginTypes.ScriptGenerator;

    protected override HashSet<Type> AcceptedPluginTypes { get; } = new()
    {
        typeof(IScriptGeneratorStrategy)
    };

    public ScriptGeneratorPluginLoader(IFileService fileService, IDllLoader dllLoader,
        IPluginRepository pluginRepository, IPluginService pluginService) : base(fileService, dllLoader,
        pluginRepository, pluginService)
    {
    }
}
