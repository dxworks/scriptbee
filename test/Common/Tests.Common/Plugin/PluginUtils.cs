using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Tests.Common.Plugin;

public static class PluginUtils
{
    public sealed record TestBundlePlugin(string Kind, string Id, string Version);

    public static Domain.Model.Plugin.Plugin CreateBundlePlugin(
        string bundleId,
        string bundleVersion,
        params TestBundlePlugin[] plugins
    )
    {
        var plugin = new TestPlugin(bundleId, new Version(bundleVersion))
        {
            Manifest = { ExtensionPoints = new List<PluginExtensionPoint>() },
        };

        foreach (var (kind, id, version) in plugins)
        {
            plugin.Manifest.ExtensionPoints.Add(
                new TestPluginExtensionPoint
                {
                    Kind = kind,
                    EntryPoint = id,
                    Version = version,
                }
            );
        }

        return plugin;
    }
}
