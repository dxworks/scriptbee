using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Tests.Plugin.Internals;

public record TestPlugin(string FolderPath = "path") : Models.Plugin(FolderPath, new TestPluginManifest());

public class TestPluginManifest : PluginManifest
{
}
