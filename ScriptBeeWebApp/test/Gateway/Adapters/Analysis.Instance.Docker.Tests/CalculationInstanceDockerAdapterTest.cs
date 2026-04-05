using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.Analysis.Instance.Docker.Config;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;

namespace ScriptBee.Analysis.Instance.Docker.Tests;

public class CalculationInstanceDockerAdapterTest : IClassFixture<DockerFixture>
{
    private readonly IFreePortProvider _freePortProvider = Substitute.For<IFreePortProvider>();
    private readonly DockerFixture _dockerFixture;
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly ILogger<CalculationInstanceDockerAdapter> _logger;
    private readonly IOptions<CalculationDockerConfig> _configOptions;

    private readonly int _testPort;

    private const string TestMongoConnectionString = "mongodb://test:27017";
    private const string OverrideMongoConnectionString = "mongodb://mongodb-host:27017";

    public CalculationInstanceDockerAdapterTest(
        DockerFixture dockerFixture,
        ITestOutputHelper outputHelper
    )
    {
        _dockerFixture = dockerFixture;
        var config = new CalculationDockerConfig
        {
            DockerSocket = dockerFixture.DockerClient.Configuration.EndpointBaseUri.ToString(),
            Network = DockerFixture.TestNetworkName,
        };
        _configOptions = Options.Create(config);

        _testPort = new FreePortProvider().GetFreeTcpPort();
        _freePortProvider.GetFreeTcpPort().Returns(_testPort);

        _configuration
            .GetSection("ConnectionStrings")["mongodb"]
            .Returns(TestMongoConnectionString);

        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new XUnitLoggerProvider(outputHelper))
        );
        _logger = loggerFactory.CreateLogger<CalculationInstanceDockerAdapter>();
    }

    [Fact]
    public async Task Allocate_ShouldCreateAndStartContainerAndReturnUrlWithNetworkIP()
    {
        // Arrange
        var adapter = new CalculationInstanceDockerAdapter(
            _configOptions,
            _configuration,
            _logger,
            _freePortProvider
        );
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(ProjectId.FromValue("id"));
        var instanceId = new InstanceId(Guid.NewGuid());
        var image = new AnalysisInstanceImage(DockerFixture.TestImageName);

        // Act
        var containerUrl = await adapter.Allocate(
            projectDetails,
            instanceId,
            image,
            TestContext.Current.CancellationToken
        );

        // Assert
        containerUrl.ShouldStartWith("http://");
        containerUrl.ShouldContain($":{_testPort}");
        var containers = await _dockerFixture.DockerClient.Containers.ListContainersAsync(
            new ContainersListParameters { All = true },
            TestContext.Current.CancellationToken
        );
        var ourContainer = containers.FirstOrDefault(c =>
            c.Names.Contains($"/scriptbee-calculation-{instanceId}")
        );
        ourContainer.ShouldNotBeNull();
        ourContainer.State.ShouldBe("running");
        ourContainer.NetworkSettings.Networks.ShouldContainKey(DockerFixture.TestNetworkName);
        ourContainer
            .NetworkSettings.Networks[DockerFixture.TestNetworkName]
            .IPAddress.ShouldNotBeNullOrEmpty();
        containerUrl.ShouldBe(
            $"http://{ourContainer.NetworkSettings.Networks[DockerFixture.TestNetworkName].IPAddress}:{_testPort}"
        );
    }

    [Fact]
    public async Task Allocate_ShouldUseConfiguredNetworkAndContainerName()
    {
        // Arrange
        var adapter = new CalculationInstanceDockerAdapter(
            _configOptions,
            _configuration,
            _logger,
            _freePortProvider
        );
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(ProjectId.FromValue("id"));
        var instanceId = new InstanceId(Guid.NewGuid());
        var image = new AnalysisInstanceImage(DockerFixture.TestImageName);

        // Act
        await adapter.Allocate(
            projectDetails,
            instanceId,
            image,
            TestContext.Current.CancellationToken
        );

        // Assert
        var containers = await _dockerFixture.DockerClient.Containers.ListContainersAsync(
            new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {
                        "name",
                        new Dictionary<string, bool>
                        {
                            { $"scriptbee-calculation-{instanceId}", true },
                        }
                    },
                },
            },
            TestContext.Current.CancellationToken
        );
        containers.ShouldHaveSingleItem();
        containers.First().Names.ShouldContain($"/scriptbee-calculation-{instanceId}");
        containers.First().NetworkSettings.Networks.ShouldContainKey(DockerFixture.TestNetworkName);
    }

    [Fact]
    public async Task Allocate_ShouldPassEnvironmentVariables()
    {
        // Arrange
        var adapter = new CalculationInstanceDockerAdapter(
            _configOptions,
            _configuration,
            _logger,
            _freePortProvider
        );
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(
            ProjectId.FromValue("id")
        ) with
        {
            Name = "project-name",
        };
        var instanceId = new InstanceId(Guid.NewGuid());
        var image = new AnalysisInstanceImage(DockerFixture.TestImageName);

        // Act
        await adapter.Allocate(
            projectDetails,
            instanceId,
            image,
            TestContext.Current.CancellationToken
        );

        // Assert
        var containerInspect = await _dockerFixture.DockerClient.Containers.InspectContainerAsync(
            $"scriptbee-calculation-{instanceId}",
            TestContext.Current.CancellationToken
        );

        containerInspect.Config.Env.ShouldContain($"ScriptBee__InstanceId={instanceId}");
        containerInspect.Config.Env.ShouldContain("ScriptBee__ProjectId=id");
        containerInspect.Config.Env.ShouldContain("ScriptBee__ProjectName=project-name");
        containerInspect.Config.Env.ShouldContain(
            $"ConnectionStrings__mongodb={TestMongoConnectionString}"
        );
    }

    [Fact]
    public async Task Allocate_ShouldUseOverrideMongoDbConnectionString_WhenConfigured()
    {
        // Arrange
        var config = new CalculationDockerConfig
        {
            DockerSocket = _dockerFixture.DockerClient.Configuration.EndpointBaseUri.ToString(),
            Network = DockerFixture.TestNetworkName,
            MongoDbConnectionString = OverrideMongoConnectionString,
        };
        var adapter = new CalculationInstanceDockerAdapter(
            Options.Create(config),
            _configuration,
            _logger,
            _freePortProvider
        );
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(ProjectId.FromValue("id"));
        var instanceId = new InstanceId(Guid.NewGuid());
        var image = new AnalysisInstanceImage(DockerFixture.TestImageName);

        // Act
        await adapter.Allocate(
            projectDetails,
            instanceId,
            image,
            TestContext.Current.CancellationToken
        );

        // Assert
        var containerInspect = await _dockerFixture.DockerClient.Containers.InspectContainerAsync(
            $"scriptbee-calculation-{instanceId}",
            TestContext.Current.CancellationToken
        );

        containerInspect.Config.Env.ShouldContain(
            $"ConnectionStrings__mongodb={OverrideMongoConnectionString}"
        );
    }

    [Fact]
    public async Task Deallocate_ShouldStopAndRemoveExistingContainer()
    {
        // Arrange
        var adapter = new CalculationInstanceDockerAdapter(
            _configOptions,
            _configuration,
            _logger,
            _freePortProvider
        );
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(ProjectId.FromValue("id"));
        var instanceId = new InstanceId(Guid.NewGuid());
        var instanceImage = new AnalysisInstanceImage(DockerFixture.TestImageName);
        var containerName = $"scriptbee-calculation-{instanceId}";

        var instanceUrl = await adapter.Allocate(
            projectDetails,
            instanceId,
            instanceImage,
            TestContext.Current.CancellationToken
        );
        instanceUrl.ShouldStartWith("http://");
        var instanceInfo = new InstanceInfo(
            instanceId,
            ProjectId.FromValue("project-id"),
            instanceUrl,
            DateTimeOffset.UtcNow,
            CalculationInstanceStatus.NotFound
        );

        // Act
        await adapter.Deallocate(instanceInfo, TestContext.Current.CancellationToken);

        // Assert
        var containers = await _dockerFixture.DockerClient.Containers.ListContainersAsync(
            new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {
                        "name",
                        new Dictionary<string, bool> { { containerName, true } }
                    },
                },
            },
            TestContext.Current.CancellationToken
        );
        containers.ShouldBeEmpty();
    }

    [Fact]
    public async Task Deallocate_ShouldNotThrowException_IfContainerNotFound()
    {
        var adapter = new CalculationInstanceDockerAdapter(
            _configOptions,
            _configuration,
            _logger,
            _freePortProvider
        );
        var instanceId = new InstanceId(Guid.NewGuid());
        var instanceInfo = new InstanceInfo(
            instanceId,
            ProjectId.FromValue("project-id"),
            "http://fakeurl",
            DateTimeOffset.UtcNow,
            CalculationInstanceStatus.NotFound
        );

        var exception = await Record.ExceptionAsync(() =>
            adapter.Deallocate(instanceInfo, TestContext.Current.CancellationToken)
        );

        exception.ShouldBeNull();
    }
}
