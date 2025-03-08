using ScriptBee.Common;

namespace ScriptBee.Analysis.Web.Extensions;

public static class CommonServicesExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IGuidProvider, GuidProvider>();
    }
}
