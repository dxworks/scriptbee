using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Analysis.Instance.Docker.Config;
using ScriptBee.Ports.Instance.Allocation;

namespace ScriptBee.Analysis.Instance.Docker.Extensions;

public static class AnalysisDockerInstanceExtensions
{
    public static IServiceCollection AddDockerInstanceAdapter(
        this IServiceCollection services,
        string dockerConfigSection
    )
    {
        services.AddOptions<AnalysisDockerConfig>().BindConfiguration("ScriptBee:Analysis:Docker");

        return services
            .AddSingleton<IFreePortProvider, FreePortProvider>()
            .AddSingleton<IAllocateInstance, AnalysisInstanceDockerAdapter>()
            .AddSingleton<IDeallocateInstance, AnalysisInstanceDockerAdapter>()
            .AddSingleton<IGetInstanceStatus, AnalysisInstanceDockerAdapter>();
    }
}
