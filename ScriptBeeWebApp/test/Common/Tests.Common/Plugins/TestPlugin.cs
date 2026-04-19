using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Tests.Common.Plugins;

public record TestPlugin(PluginId Id, string FolderPath = "path")
    : Plugin(
        FolderPath,
        Id,
        new TestPluginManifest { ExtensionPoints = [new TestPluginExtensionPoint()] }
    );

public class TestPluginManifest : PluginManifest;

public class TestPluginExtensionPoint : PluginExtensionPoint;
