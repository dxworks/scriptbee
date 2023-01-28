using DxWorks.Hub.Sdk;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Marketplace.Client.Services;

namespace ScriptBee.Marketplace.Client;

public static class ScriptBeeMarketplaceClientExtensions
{
    public static IServiceCollection AddScriptBeeMarketplaceClient(this IServiceCollection services,
        Action<DxWorksHubSdkOptions>? configureOptionsAction = null)
    {
        services.AddDxWorksHubSdk(configureOptionsAction);
        services.AddSingleton<IMarketPluginFetcher, MarketPluginFetcher>();

        return services;
    }
}
