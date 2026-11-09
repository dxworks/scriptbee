using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.Analysis.Instance.Kubernetes.Config;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;

namespace ScriptBee.Analysis.Instance.Kubernetes.Tests;

[Trait("Category", "Integration")]
public class AnalysisInstanceKubernetesIntegrationTest : IClassFixture<K3sFixture>
{
    private readonly K3sFixture _k3sFixture;
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly ILogger<AnalysisInstanceKubernetesAdapter> _logger;

    public AnalysisInstanceKubernetesIntegrationTest(
        K3sFixture k3sFixture,
        ITestOutputHelper outputHelper
    )
    {
        _k3sFixture = k3sFixture;
        _configuration
            .GetSection("ConnectionStrings")["mongodb"]
            .Returns("mongodb://localhost:27017/ScriptBee");

        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new XUnitLoggerProvider(outputHelper))
        );
        _logger = loggerFactory.CreateLogger<AnalysisInstanceKubernetesAdapter>();
    }

    [Fact]
    public async Task AllocateAndDeallocate_ShouldManageActualWorkloadInKubernetes()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig { Namespace = "default", Port = 80 };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _k3sFixture.ClientFactory,
            options,
            _configuration,
            _logger
        );

        var instanceId = new InstanceId(Guid.NewGuid());
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(
            ProjectId.FromValue("integration-proj")
        );
        var image = new AnalysisInstanceImage("busybox:latest");

        // Act - Allocate
        var url = await adapter.Allocate(
            projectDetails,
            instanceId,
            image,
            TestContext.Current.CancellationToken
        );

        // Assert - Allocate
        url.ShouldBe(
            $"http://scriptbee-analysis-{instanceId.Value.ToString().ToLowerInvariant()}.default.svc.cluster.local:80"
        );

        var resourceName = $"scriptbee-analysis-{instanceId.Value.ToString().ToLowerInvariant()}";
        var pod = await _k3sFixture.Client.CoreV1.ReadNamespacedPodAsync(
            resourceName,
            "default",
            cancellationToken: TestContext.Current.CancellationToken
        );
        pod.ShouldNotBeNull();
        pod.Metadata.Name.ShouldBe(resourceName);

        var service = await _k3sFixture.Client.CoreV1.ReadNamespacedServiceAsync(
            resourceName,
            "default",
            cancellationToken: TestContext.Current.CancellationToken
        );
        service.ShouldNotBeNull();
        service.Metadata.Name.ShouldBe(resourceName);

        // Act - Status
        var status = await adapter.GetStatus(instanceId, TestContext.Current.CancellationToken);

        // Assert - Status
        status.ShouldBeOneOf(AnalysisInstanceStatus.Allocating, AnalysisInstanceStatus.Running);

        // Act - Deallocate
        var instanceInfo = new InstanceInfo(
            instanceId,
            projectDetails.Id,
            url,
            DateTime.UtcNow,
            AnalysisInstanceStatus.Running
        );
        await adapter.Deallocate(instanceInfo, TestContext.Current.CancellationToken);

        // Assert - Deallocate
        var statusAfterDeallocate = await adapter.GetStatus(
            instanceId,
            TestContext.Current.CancellationToken
        );
        statusAfterDeallocate.ShouldBeOneOf(
            AnalysisInstanceStatus.Deallocating,
            AnalysisInstanceStatus.NotFound
        );
    }
}
