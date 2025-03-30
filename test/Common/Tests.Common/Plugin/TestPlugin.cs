using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Tests.Common.Plugin;

public record TestPlugin(string Id, Version Version, string FolderPath = "path")
    : Domain.Model.Plugin.Plugin(
        FolderPath,
        Id,
        Version,
        new TestPluginManifest { ExtensionPoints = [new TestPluginExtensionPoint()] }
    );

public class TestPluginManifest : PluginManifest;

public class TestPluginExtensionPoint : PluginExtensionPoint;
