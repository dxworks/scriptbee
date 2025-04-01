using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Analysis.Instance.Docker.Config;
using ScriptBee.Ports.Instance;

namespace ScriptBee.Analysis.Instance.Docker.Extensions;

public static class AnalysisDockerInstanceExtensions
{
    public static IServiceCollection AddDockerInstanceAdapter(
        this IServiceCollection services,
        string dockerConfigSection
    )
    {
        services
            .AddOptions<CalculationDockerConfig>()
            .BindConfiguration("ScriptBee:Calculation:Docker");

        return services
            .AddSingleton<IFreePortProvider, FreePortProvider>()
            .AddSingleton<IAllocateInstance, CalculationInstanceDockerAdapter>()
            .AddSingleton<IDeallocateInstance, CalculationInstanceDockerAdapter>();
    }
}
