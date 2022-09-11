﻿using System;
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
            { "ScriptGenerator", typeof(ScriptGeneratorPluginManifest) },
            { "UI", typeof(UiPluginManifest) }
        };
    }
}
