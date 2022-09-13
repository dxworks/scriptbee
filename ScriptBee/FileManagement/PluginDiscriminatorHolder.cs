using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.FileManagement;

// todo make it more extensible
public class PluginDiscriminatorHolder : IPluginDiscriminatorHolder
{
    public Dictionary<string, Type> GetDiscriminatedTypes()
    {
        return new Dictionary<string, Type>
        {
            { PluginTypes.ScriptGenerator, typeof(ScriptGeneratorPluginManifest) },
            { PluginTypes.ScriptRunner, typeof(ScriptRunnerPluginManifest) },
            { PluginTypes.HelperFunctions, typeof(HelperFunctionsPluginManifest) },
            { PluginTypes.Ui, typeof(UiPluginManifest) },
        };
    }
}
