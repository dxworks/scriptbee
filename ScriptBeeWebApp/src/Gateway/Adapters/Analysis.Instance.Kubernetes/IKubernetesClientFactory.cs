using k8s;

namespace ScriptBee.Analysis.Instance.Kubernetes;

public interface IKubernetesClientFactory
{
    IKubernetes CreateClient();
}
