using System.Collections.Generic;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Tests.Plugin.Internals;

public record TestPlugin(string FolderPath = "path") : Models.Plugin(FolderPath, new TestPluginManifest
{
    ExtensionPoints = new List<PluginExtensionPoint> { new TestPluginExtensionPoint() }
});

public class TestPluginManifest : PluginManifest
{
}

public class TestPluginExtensionPoint : PluginExtensionPoint
{
}
