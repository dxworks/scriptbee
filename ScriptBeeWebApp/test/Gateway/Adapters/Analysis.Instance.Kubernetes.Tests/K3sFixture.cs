using System.Text;
using k8s;
using NSubstitute;
using Testcontainers.K3s;

namespace ScriptBee.Analysis.Instance.Kubernetes.Tests;

public class K3sFixture : IAsyncLifetime
{
    private readonly K3sContainer _container = new K3sBuilder("rancher/k3s:v1.31.2-k3s1").Build();

    public IKubernetes Client { get; private set; } = null!;

    public IKubernetesClientFactory ClientFactory { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        var kubeconfig = await _container.GetKubeconfigAsync();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(kubeconfig));
        var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(stream);

        Client = new k8s.Kubernetes(config);

        var factory = Substitute.For<IKubernetesClientFactory>();
        factory.CreateClient().Returns(_ => new k8s.Kubernetes(config));
        ClientFactory = factory;
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await _container.DisposeAsync();
    }
}
