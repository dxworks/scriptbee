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
            { PluginKinds.ScriptGenerator, typeof(ScriptGeneratorPluginManifest) },
            { PluginKinds.ScriptRunner, typeof(ScriptRunnerPluginManifest) },
            { PluginKinds.HelperFunctions, typeof(HelperFunctionsPluginManifest) },
            { PluginKinds.Loader, typeof(LoaderPluginManifest) },
            { PluginKinds.Linker, typeof(LinkerPluginManifest) },
            { PluginKinds.Ui, typeof(UiPluginManifest) },
        };
    }
}
