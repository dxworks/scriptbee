using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Tests.Common.Plugin;

public record TestPlugin(PluginId Id, string FolderPath = "path")
    : Domain.Model.Plugins.Plugin(
        FolderPath,
        Id,
        new TestPluginManifest { ExtensionPoints = [new TestPluginExtensionPoint()] }
    );

public class TestPluginManifest : PluginManifest;

public class TestPluginExtensionPoint : PluginExtensionPoint;
