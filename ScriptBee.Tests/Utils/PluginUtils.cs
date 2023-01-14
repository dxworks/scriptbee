using System;
using System.Collections.Generic;
using ScriptBee.Plugin.Manifest;
using ScriptBee.Tests.Plugin.Internals;

namespace ScriptBee.Tests.Utils;

internal static class PluginUtils
{
    internal sealed record TestBundlePlugin(string Kind, string Id, string Version);

    internal static Models.Plugin CreateBundlePlugin(string bundleId, string bundleVersion,
        params TestBundlePlugin[] plugins)
    {
        var plugin = new TestPlugin(bundleId, new Version(bundleVersion))
        {
            Manifest =
            {
                ExtensionPoints = new List<PluginExtensionPoint>()
            }
        };

        foreach (var (kind, id, version) in plugins)
        {
            plugin.Manifest.ExtensionPoints.Add(new TestPluginExtensionPoint
            {
                Kind = kind,
                EntryPoint = id,
                Version = version
            });
        }

        return plugin;
    }
}
