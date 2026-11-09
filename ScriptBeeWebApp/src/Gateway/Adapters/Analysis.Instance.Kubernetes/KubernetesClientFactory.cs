using k8s;
using Microsoft.Extensions.Options;
using ScriptBee.Analysis.Instance.Kubernetes.Config;

namespace ScriptBee.Analysis.Instance.Kubernetes;

public sealed class KubernetesClientFactory(IOptions<AnalysisKubernetesConfig> options)
    : IKubernetesClientFactory
{
    public IKubernetes CreateClient()
    {
        var config = options.Value;

        var k8sConfig = !string.IsNullOrWhiteSpace(config.KubeConfigPath)
            ? KubernetesClientConfiguration.BuildConfigFromConfigFile(config.KubeConfigPath)
            : KubernetesClientConfiguration.BuildDefaultConfig();

        return new k8s.Kubernetes(k8sConfig);
    }
}
