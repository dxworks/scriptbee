﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.EndpointDefinitions.DTO;

namespace ScriptBeeWebApp.Services;

public interface IPluginService
{
    IEnumerable<PluginManifest> GetPluginManifests();
    IEnumerable<PluginManifest> GetPluginManifests(string type);
    IEnumerable<T> GetExtensionPoints<T>() where T : PluginExtensionPoint;

    Task<IEnumerable<MarketplaceProject>> GetMarketPlugins(CancellationToken cancellationToken = default);

    Task InstallPlugin(string pluginId, string version, CancellationToken cancellationToken = default);
    void UninstallPlugin(string pluginId, string pluginVersion);
}
