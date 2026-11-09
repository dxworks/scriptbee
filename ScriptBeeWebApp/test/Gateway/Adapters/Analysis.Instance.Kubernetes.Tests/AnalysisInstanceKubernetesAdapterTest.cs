using System.Net;
using k8s;
using k8s.Autorest;
using k8s.Models;
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

public class AnalysisInstanceKubernetesAdapterTest
{
    private readonly IKubernetesClientFactory _clientFactory =
        Substitute.For<IKubernetesClientFactory>();
    private readonly IKubernetes _kubernetesClient = Substitute.For<IKubernetes>();
    private readonly ICoreV1Operations _coreV1 = Substitute.For<ICoreV1Operations>();
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly ILogger<AnalysisInstanceKubernetesAdapter> _logger;

    private const string TestMongoConnectionString = "mongodb://mongodb-service:27017/ScriptBee";
    private const string TestNamespace = "scriptbee-test";
    private const int TestPort = 8080;

    public AnalysisInstanceKubernetesAdapterTest(ITestOutputHelper outputHelper)
    {
        _kubernetesClient.CoreV1.Returns(_coreV1);
        _clientFactory.CreateClient().Returns(_kubernetesClient);

        _configuration
            .GetSection("ConnectionStrings")["mongodb"]
            .Returns(TestMongoConnectionString);

        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new XUnitLoggerProvider(outputHelper))
        );
        _logger = loggerFactory.CreateLogger<AnalysisInstanceKubernetesAdapter>();
    }

    [Fact]
    public async Task Allocate_ShouldCreatePodAndServiceWithProperSpecs_AndReturnClusterFqdn()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig
        {
            Namespace = TestNamespace,
            Port = TestPort,
            Labels = new Dictionary<string, string> { ["env"] = "testing" },
        };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );

        var projectId = ProjectId.FromValue("proj-1");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);
        var instanceId = new InstanceId(Guid.Parse("11111111-2222-3333-4444-555555555555"));
        var image = new AnalysisInstanceImage("dxworks/scriptbee-analysis:latest");

        V1Pod? createdPod = null;
        V1Service? createdService = null;

        _coreV1
            .CreateNamespacedPodWithHttpMessagesAsync(
                Arg.Do<V1Pod>(p => createdPod = p),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Pod>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.Created),
                        Body = new V1Pod(),
                    }
                )
            );

        _coreV1
            .CreateNamespacedServiceWithHttpMessagesAsync(
                Arg.Do<V1Service>(s => createdService = s),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Service>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.Created),
                        Body = new V1Service(),
                    }
                )
            );

        // Act
        var url = await adapter.Allocate(
            projectDetails,
            instanceId,
            image,
            TestContext.Current.CancellationToken
        );

        // Assert
        url.ShouldBe(
            $"http://scriptbee-analysis-11111111-2222-3333-4444-555555555555.{TestNamespace}.svc.cluster.local:{TestPort}"
        );

        createdPod.ShouldNotBeNull();
        createdPod.Metadata.Name.ShouldBe(
            "scriptbee-analysis-11111111-2222-3333-4444-555555555555"
        );
        createdPod.Metadata.NamespaceProperty.ShouldBe(TestNamespace);
        createdPod.Metadata.Labels["app.kubernetes.io/name"].ShouldBe("scriptbee-analysis");
        createdPod
            .Metadata.Labels["scriptbee.org/instance-id"]
            .ShouldBe("11111111-2222-3333-4444-555555555555");
        createdPod.Metadata.Labels["scriptbee.org/project-id"].ShouldBe("proj-1");
        createdPod.Metadata.Labels["env"].ShouldBe("testing");

        var container = createdPod.Spec.Containers.ShouldHaveSingleItem();
        container.Name.ShouldBe("analysis");
        container.Image.ShouldBe("dxworks/scriptbee-analysis:latest");
        container.Ports.ShouldHaveSingleItem().ContainerPort.ShouldBe(TestPort);
        container.ReadinessProbe.ShouldNotBeNull();
        container.ReadinessProbe.HttpGet.Path.ShouldBe("/health");

        container.Env.ShouldContain(e =>
            e.Name == "ScriptBee__InstanceId" && e.Value == instanceId.Value.ToString()
        );
        container.Env.ShouldContain(e => e.Name == "ScriptBee__ProjectId" && e.Value == "proj-1");
        container.Env.ShouldContain(e =>
            e.Name == "ConnectionStrings__mongodb" && e.Value == TestMongoConnectionString
        );
        container.Env.ShouldContain(e =>
            e.Name == "ASPNETCORE_HTTP_PORTS" && e.Value == TestPort.ToString()
        );

        createdService.ShouldNotBeNull();
        createdService.Metadata.Name.ShouldBe(
            "scriptbee-analysis-11111111-2222-3333-4444-555555555555"
        );
        createdService.Metadata.NamespaceProperty.ShouldBe(TestNamespace);
        createdService.Spec.Type.ShouldBe("ClusterIP");
        createdService
            .Spec.Selector["scriptbee.org/instance-id"]
            .ShouldBe("11111111-2222-3333-4444-555555555555");
        createdService.Spec.Ports.ShouldHaveSingleItem().Port.ShouldBe(TestPort);
    }

    [Fact]
    public async Task Allocate_ShouldAttachPvcVolumes_WhenPvcNamesAreConfigured()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig
        {
            Namespace = TestNamespace,
            Port = TestPort,
            UserFolderVolumePath = "/root/.scriptbee",
            UserFolderPvcName = "user-data-pvc",
            PluginsPvcName = "plugins-pvc",
        };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );

        var projectId = ProjectId.FromValue("proj-2");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);
        var instanceId = new InstanceId(Guid.NewGuid());
        var image = new AnalysisInstanceImage("dxworks/scriptbee-analysis:latest");

        V1Pod? createdPod = null;
        _coreV1
            .CreateNamespacedPodWithHttpMessagesAsync(
                Arg.Do<V1Pod>(p => createdPod = p),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Pod>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.Created),
                        Body = new V1Pod(),
                    }
                )
            );

        _coreV1
            .CreateNamespacedServiceWithHttpMessagesAsync(
                Arg.Any<V1Service>(),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Service>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.Created),
                        Body = new V1Service(),
                    }
                )
            );

        // Act
        await adapter.Allocate(
            projectDetails,
            instanceId,
            image,
            TestContext.Current.CancellationToken
        );

        // Assert
        createdPod.ShouldNotBeNull();
        var container = createdPod.Spec.Containers.ShouldHaveSingleItem();
        container.VolumeMounts.Count.ShouldBe(2);
        container.VolumeMounts.ShouldContain(v =>
            v.Name == "user-folder" && v.MountPath == "/root/.scriptbee"
        );
        container.VolumeMounts.ShouldContain(v =>
            v.Name == "plugins-volume"
            && v.MountPath == "/app/plugins"
            && v.ReadOnlyProperty == true
        );

        createdPod.Spec.Volumes.Count.ShouldBe(2);
        createdPod.Spec.Volumes.ShouldContain(v =>
            v.Name == "user-folder" && v.PersistentVolumeClaim.ClaimName == "user-data-pvc"
        );
        createdPod.Spec.Volumes.ShouldContain(v =>
            v.Name == "plugins-volume" && v.PersistentVolumeClaim.ClaimName == "plugins-pvc"
        );
    }

    [Fact]
    public async Task Allocate_ShouldApplyResourceRequirementsNodeSelectorAndTolerations_WhenConfigured()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig
        {
            Namespace = TestNamespace,
            Port = TestPort,
            Resources = new PodResourceRequirements
            {
                Requests = new Dictionary<string, string>
                {
                    ["cpu"] = "100m",
                    ["memory"] = "256Mi",
                },
                Limits = new Dictionary<string, string> { ["cpu"] = "500m", ["memory"] = "1Gi" },
            },
            NodeSelector = new Dictionary<string, string> { ["disktype"] = "ssd" },
            Tolerations = new List<PodTolerationConfig>
            {
                new()
                {
                    Key = "dedicated",
                    Operator = "Equal",
                    Value = "analysis",
                    Effect = "NoSchedule",
                },
            },
        };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );

        var projectId = ProjectId.FromValue("proj-3");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);
        var instanceId = new InstanceId(Guid.NewGuid());
        var image = new AnalysisInstanceImage("dxworks/scriptbee-analysis:latest");

        V1Pod? createdPod = null;
        _coreV1
            .CreateNamespacedPodWithHttpMessagesAsync(
                Arg.Do<V1Pod>(p => createdPod = p),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Pod>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.Created),
                        Body = new V1Pod(),
                    }
                )
            );

        _coreV1
            .CreateNamespacedServiceWithHttpMessagesAsync(
                Arg.Any<V1Service>(),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Service>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.Created),
                        Body = new V1Service(),
                    }
                )
            );

        // Act
        await adapter.Allocate(
            projectDetails,
            instanceId,
            image,
            TestContext.Current.CancellationToken
        );

        // Assert
        createdPod.ShouldNotBeNull();
        var container = createdPod.Spec.Containers.ShouldHaveSingleItem();
        container.Resources.ShouldNotBeNull();
        container.Resources.Requests["cpu"].ToString().ShouldBe("100m");
        container.Resources.Requests["memory"].ToString().ShouldBe("256Mi");
        container.Resources.Limits["cpu"].ToString().ShouldBe("500m");
        container.Resources.Limits["memory"].ToString().ShouldBe("1Gi");

        createdPod.Spec.NodeSelector["disktype"].ShouldBe("ssd");
        var toleration = createdPod.Spec.Tolerations.ShouldHaveSingleItem();
        toleration.Key.ShouldBe("dedicated");
        toleration.OperatorProperty.ShouldBe("Equal");
        toleration.Value.ShouldBe("analysis");
        toleration.Effect.ShouldBe("NoSchedule");
    }

    [Fact]
    public async Task Deallocate_ShouldDeletePodAndService()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig { Namespace = TestNamespace };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );

        var instanceId = new InstanceId(Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"));
        var instanceInfo = new InstanceInfo(
            instanceId,
            ProjectId.FromValue("p1"),
            "http://dummy",
            DateTime.UtcNow,
            AnalysisInstanceStatus.Running
        );

        _coreV1
            .DeleteNamespacedPodWithHttpMessagesAsync(
                "scriptbee-analysis-aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Pod>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.OK),
                        Body = new V1Pod(),
                    }
                )
            );

        _coreV1
            .DeleteNamespacedServiceWithHttpMessagesAsync(
                "scriptbee-analysis-aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Service>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.OK),
                        Body = new V1Service(),
                    }
                )
            );

        // Act
        await adapter.Deallocate(instanceInfo, TestContext.Current.CancellationToken);

        // Assert
        await _coreV1
            .Received(1)
            .DeleteNamespacedPodWithHttpMessagesAsync(
                "scriptbee-analysis-aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            );
        await _coreV1
            .Received(1)
            .DeleteNamespacedServiceWithHttpMessagesAsync(
                "scriptbee-analysis-aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Deallocate_ShouldHandleNotFoundExceptionGracefully()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig { Namespace = TestNamespace };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );

        var instanceId = new InstanceId(Guid.NewGuid());
        var instanceInfo = new InstanceInfo(
            instanceId,
            ProjectId.FromValue("p1"),
            "http://dummy",
            DateTime.UtcNow,
            AnalysisInstanceStatus.Running
        );

        var notFoundException = new HttpOperationException
        {
            Response = new HttpResponseMessageWrapper(
                new HttpResponseMessage(HttpStatusCode.NotFound),
                ""
            ),
        };

        _coreV1
            .DeleteNamespacedPodWithHttpMessagesAsync(
                Arg.Any<string>(),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns<Task<HttpOperationResponse<V1Pod>>>(_ => throw notFoundException);

        _coreV1
            .DeleteNamespacedServiceWithHttpMessagesAsync(
                Arg.Any<string>(),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns<Task<HttpOperationResponse<V1Service>>>(_ => throw notFoundException);

        // Act & Assert
        await Should.NotThrowAsync(async () =>
            await adapter.Deallocate(instanceInfo, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    public async Task GetStatus_ShouldReturnAllocating_WhenPodIsPending()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig { Namespace = TestNamespace };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );
        var instanceId = new InstanceId(Guid.NewGuid());

        var pod = new V1Pod { Status = new V1PodStatus { Phase = "Pending" } };

        _coreV1
            .ReadNamespacedPodStatusWithHttpMessagesAsync(
                Arg.Any<string>(),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Pod>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.OK),
                        Body = pod,
                    }
                )
            );

        // Act
        var status = await adapter.GetStatus(instanceId, TestContext.Current.CancellationToken);

        // Assert
        status.ShouldBe(AnalysisInstanceStatus.Allocating);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnAllocating_WhenPodIsRunningButNotReady()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig { Namespace = TestNamespace };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );
        var instanceId = new InstanceId(Guid.NewGuid());

        var pod = new V1Pod
        {
            Status = new V1PodStatus
            {
                Phase = "Running",
                Conditions = new List<V1PodCondition>
                {
                    new() { Type = "Ready", Status = "False" },
                },
            },
        };

        _coreV1
            .ReadNamespacedPodStatusWithHttpMessagesAsync(
                Arg.Any<string>(),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Pod>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.OK),
                        Body = pod,
                    }
                )
            );

        // Act
        var status = await adapter.GetStatus(instanceId, TestContext.Current.CancellationToken);

        // Assert
        status.ShouldBe(AnalysisInstanceStatus.Allocating);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnRunning_WhenPodIsRunningAndReady()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig { Namespace = TestNamespace };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );
        var instanceId = new InstanceId(Guid.NewGuid());

        var pod = new V1Pod
        {
            Status = new V1PodStatus
            {
                Phase = "Running",
                Conditions = new List<V1PodCondition>
                {
                    new() { Type = "Ready", Status = "True" },
                },
            },
        };

        _coreV1
            .ReadNamespacedPodStatusWithHttpMessagesAsync(
                Arg.Any<string>(),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Pod>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.OK),
                        Body = pod,
                    }
                )
            );

        // Act
        var status = await adapter.GetStatus(instanceId, TestContext.Current.CancellationToken);

        // Assert
        status.ShouldBe(AnalysisInstanceStatus.Running);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnDeallocating_WhenPodHasDeletionTimestamp()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig { Namespace = TestNamespace };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );
        var instanceId = new InstanceId(Guid.NewGuid());

        var pod = new V1Pod
        {
            Metadata = new V1ObjectMeta { DeletionTimestamp = DateTime.UtcNow },
            Status = new V1PodStatus { Phase = "Running" },
        };

        _coreV1
            .ReadNamespacedPodStatusWithHttpMessagesAsync(
                Arg.Any<string>(),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(
                    new HttpOperationResponse<V1Pod>
                    {
                        Response = new HttpResponseMessage(HttpStatusCode.OK),
                        Body = pod,
                    }
                )
            );

        // Act
        var status = await adapter.GetStatus(instanceId, TestContext.Current.CancellationToken);

        // Assert
        status.ShouldBe(AnalysisInstanceStatus.Deallocating);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnNotFound_WhenPodNotFoundOrFailed()
    {
        // Arrange
        var config = new AnalysisKubernetesConfig { Namespace = TestNamespace };
        var options = Options.Create(config);
        var adapter = new AnalysisInstanceKubernetesAdapter(
            _clientFactory,
            options,
            _configuration,
            _logger
        );
        var instanceId = new InstanceId(Guid.NewGuid());

        var notFoundException = new HttpOperationException
        {
            Response = new HttpResponseMessageWrapper(
                new HttpResponseMessage(HttpStatusCode.NotFound),
                ""
            ),
        };

        _coreV1
            .ReadNamespacedPodStatusWithHttpMessagesAsync(
                Arg.Any<string>(),
                TestNamespace,
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns<Task<HttpOperationResponse<V1Pod>>>(_ => throw notFoundException);

        // Act
        var status = await adapter.GetStatus(instanceId, TestContext.Current.CancellationToken);

        // Assert
        status.ShouldBe(AnalysisInstanceStatus.NotFound);
    }
}
