using System;
using System.Collections.Generic;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.FileManagement;

// todo make it more extensible
public class PluginDiscriminatorHolder : IPluginDiscriminatorHolder
{
    public Dictionary<string, Type> GetDiscriminatedTypes()
    {
        return new Dictionary<string, Type>
        {
            { PluginKind.Plugin, typeof(PluginBundleExtensionPoint) },
            { PluginKind.ScriptGenerator, typeof(ScriptGeneratorPluginExtensionPoint) },
            { PluginKind.ScriptRunner, typeof(ScriptRunnerPluginExtensionPoint) },
            { PluginKind.HelperFunctions, typeof(HelperFunctionsPluginExtensionPoint) },
            { PluginKind.Loader, typeof(LoaderPluginExtensionPoint) },
            { PluginKind.Linker, typeof(LinkerPluginExtensionPoint) },
            { PluginKind.Ui, typeof(UiPluginExtensionPoint) }
        };
    }
}
