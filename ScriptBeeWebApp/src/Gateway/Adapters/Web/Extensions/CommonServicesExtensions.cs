using ScriptBee.Common;
using ScriptBee.Service.Project.Plugin;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Web.Extensions;

public static class CommonServicesExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IGuidProvider, GuidProvider>()
            .AddSingleton<IGetScriptAbsolutePathUseCase, GetScriptAbsolutePathService>()
            .AddSingleton<ScriptGeneratorStrategyFactory>();
    }
}
