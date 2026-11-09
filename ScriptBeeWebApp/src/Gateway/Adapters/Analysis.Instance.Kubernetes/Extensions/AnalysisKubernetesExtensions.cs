using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Analysis.Instance.Kubernetes.Config;
using ScriptBee.Ports.Instance.Allocation;

namespace ScriptBee.Analysis.Instance.Kubernetes.Extensions;

public static class AnalysisKubernetesExtensions
{
    public static IServiceCollection AddKubernetesInstanceAdapter(
        this IServiceCollection services,
        string kubernetesConfigSection
    )
    {
        services.AddOptions<AnalysisKubernetesConfig>().BindConfiguration(kubernetesConfigSection);

        return services
            .AddSingleton<IKubernetesClientFactory, KubernetesClientFactory>()
            .AddSingleton<IAllocateInstance, AnalysisInstanceKubernetesAdapter>()
            .AddSingleton<IDeallocateInstance, AnalysisInstanceKubernetesAdapter>()
            .AddSingleton<IGetInstanceStatus, AnalysisInstanceKubernetesAdapter>();
    }
}
