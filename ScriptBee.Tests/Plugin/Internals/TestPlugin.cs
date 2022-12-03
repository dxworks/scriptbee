using System;
using System.Collections.Generic;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Tests.Plugin.Internals;

public record TestPlugin(string Id, Version Version, string FolderPath = "path") : Models.Plugin(FolderPath, Id,
    Version,
    new TestPluginManifest
    {
        ExtensionPoints = new List<PluginExtensionPoint> { new TestPluginExtensionPoint() }
    });

public class TestPluginManifest : PluginManifest
{
}

public class TestPluginExtensionPoint : PluginExtensionPoint
{
}
