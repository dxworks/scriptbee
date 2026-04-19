using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.Tests.Common.Plugin;

public static class PluginUtils
{
    public sealed record TestBundlePlugin(string Kind, string Id, string Version);

    public static Domain.Model.Plugins.Plugin CreateBundlePlugin(
        string bundleName,
        string bundleVersion,
        params TestBundlePlugin[] plugins
    )
    {
        var plugin = new TestPlugin(new PluginId(bundleName, new Version(bundleVersion)))
        {
            Manifest = { ExtensionPoints = [] },
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
