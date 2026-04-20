using DxWorks.Hub.Sdk;
using Microsoft.Extensions.DependencyInjection;

namespace ScriptBee.Plugins.Marketplace.Extensions;

public static class ScriptBeeMarketplaceClientExtensions
{
    public static IServiceCollection AddScriptBeeMarketplaceClient(this IServiceCollection services)
    {
        return services
            .AddDxWorksHubSdk(options =>
            {
                options.HubDownloadFolder = Path.Combine(Path.GetTempPath(), "DxWorksHubDownloads");
            })
            .AddSingleton<IMarketPluginFetcher, MarketPluginFetcher>()
            .AddSingleton<IPluginUrlFetcher, PluginUrlFetcher>();
    }
}
