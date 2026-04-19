using ScriptBee.Application.Model.Services;
using ScriptBee.Common;
using ScriptBee.Service.Gateway.Plugins;

namespace ScriptBee.Web.Extensions;

public static class CommonServicesExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IGuidProvider, GuidProvider>()
            .AddScoped<IClientIdProvider, ClientIdProvider>()
            .AddSingleton<ScriptGeneratorStrategyFactory>();
    }
}
