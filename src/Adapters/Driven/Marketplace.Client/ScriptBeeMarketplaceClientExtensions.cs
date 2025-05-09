﻿using DxWorks.Hub.Sdk;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Ports.Plugins;

namespace ScriptBee.Marketplace.Client;

public static class ScriptBeeMarketplaceClientExtensions
{
    public static IServiceCollection AddScriptBeeMarketplaceClient(
        this IServiceCollection services,
        Action<DxWorksHubSdkOptions>? configureOptionsAction = null
    )
    {
        return services
            .AddDxWorksHubSdk(configureOptionsAction)
            .AddSingleton<IMarketPluginFetcher, MarketPluginFetcher>()
            .AddSingleton<IPluginUrlFetcher, PluginUrlFetcher>();
    }
}
