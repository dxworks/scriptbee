using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.Analysis.Instance.Docker.Config;
using ScriptBee.Application.Model.Config;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;

namespace ScriptBee.Analysis.Instance.Docker.Tests;

public class AnalysisInstanceDockerAdapterTest : IClassFixture<DockerFixture>
{
    private readonly IFreePortProvider _freePortProvider = Substitute.For<IFreePortProvider>();
    private readonly DockerFixture _dockerFixture;
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly ILogger<AnalysisInstanceDockerAdapter> _logger;
    private readonly IOptions<AnalysisDockerConfig> _configOptions;
    private readonly IOptions<UserFolderSettings> _userFolderOptions;

    private readonly int _testPort;

    private const string TestMongoConnectionString = "mongodb://test:27017";
    private const string OverrideMongoConnectionString = "mongodb://mongodb-host:27017";

    private static readonly string TestUserFolderPath = Path.Combine(
            Path.GetTempPath(),
            "scriptbee-test-userfolder"
        )
        .Replace("\\", "/");

    private static readonly string OverrideHostPath = Path.Combine(
            Path.GetTempPath(),
            "scriptbee-test-override"
        )
        .Replace("\\", "/");

    public AnalysisInstanceDockerAdapterTest(
        DockerFixture dockerFixture,
        ITestOutputHelper outputHelper
    )
    {
        _dockerFixture = dockerFixture;
        var config = new AnalysisDockerConfig
        {
            DockerSocket = dockerFixture.DockerClient.Configuration.EndpointBaseUri.ToString(),
            Network = DockerFixture.TestNetworkName,
            UserFolderVolumePath = "/root/.scriptbee",
        };
        _configOptions = Options.Create(config);
        _userFolderOptions = Options.Create(
            new UserFolderSettings { UserFolderPath = TestUserFolderPath }
        );

        _testPort = new FreePortProvider().GetFreeTcpPort();
        _freePortProvider.GetFreeTcpPort().Returns(_testPort);

        _configuration
            .GetSection("ConnectionStrings")["mongodb"]
            .Returns(TestMongoConnectionString);

        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new XUnitLoggerProvider(outputHelper))
        );
        _logger = loggerFactory.CreateLogger<AnalysisInstanceDockerAdapter>();
    }

    [Fact]
    public async Task Allocate_ShouldCreateAndStartContainerAndReturnUrlWithNetworkIP()
    {
        // Arrange
        var adapter = new AnalysisInstanceDockerAdapter(
            _configOptions,
            _userFolderOptions,
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
        containerUrl.ShouldContain($":{_configOptions.Value.Port}");
        var containers = await _dockerFixture.DockerClient.Containers.ListContainersAsync(
            new ContainersListParameters { All = true },
            TestContext.Current.CancellationToken
        );
        var ourContainer = containers.FirstOrDefault(c =>
            c.Names.Contains($"/scriptbee-analysis-{instanceId}")
        );
        ourContainer.ShouldNotBeNull();
        ourContainer.State.ShouldBe("running");
        ourContainer.NetworkSettings.Networks.ShouldContainKey(DockerFixture.TestNetworkName);
        ourContainer
            .NetworkSettings.Networks[DockerFixture.TestNetworkName]
            .IPAddress.ShouldNotBeNullOrEmpty();
        containerUrl.ShouldBe(
            $"http://{ourContainer.NetworkSettings.Networks[DockerFixture.TestNetworkName].IPAddress}:{_configOptions.Value.Port}"
        );
    }

    [Fact]
    public async Task Allocate_ShouldUseConfiguredNetworkAndContainerName()
    {
        // Arrange
        var adapter = new AnalysisInstanceDockerAdapter(
            _configOptions,
            _userFolderOptions,
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
                            { $"scriptbee-analysis-{instanceId}", true },
                        }
                    },
                },
            },
            TestContext.Current.CancellationToken
        );
        containers.ShouldHaveSingleItem();
        containers.First().Names.ShouldContain($"/scriptbee-analysis-{instanceId}");
        containers.First().NetworkSettings.Networks.ShouldContainKey(DockerFixture.TestNetworkName);
    }

    [Fact]
    public async Task Allocate_ShouldReturnLocalhostWithHostPort_WhenNoNetworkIsConfigured()
    {
        // Arrange
        var config = new AnalysisDockerConfig
        {
            DockerSocket = _dockerFixture.DockerClient.Configuration.EndpointBaseUri.ToString(),
            Network = null,
            UserFolderVolumePath = "/root/.scriptbee",
        };
        var adapter = new AnalysisInstanceDockerAdapter(
            Options.Create(config),
            _userFolderOptions,
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
        containerUrl.ShouldBe($"http://localhost:{_testPort}");
    }

    [Fact]
    public async Task Allocate_ShouldPassEnvironmentVariables()
    {
        // Arrange
        var adapter = new AnalysisInstanceDockerAdapter(
            _configOptions,
            _userFolderOptions,
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
            $"scriptbee-analysis-{instanceId}",
            TestContext.Current.CancellationToken
        );

        containerInspect.Config.Env.ShouldContain($"ScriptBee__InstanceId={instanceId}");
        containerInspect.Config.Env.ShouldContain("ScriptBee__ProjectId=id");
        containerInspect.Config.Env.ShouldContain("ScriptBee__ProjectName=project-name");
        containerInspect.Config.Env.ShouldContain(
            $"ConnectionStrings__mongodb={TestMongoConnectionString}"
        );
        containerInspect.Config.Env.ShouldContain(
            $"UserFolder__UserFolderPath={_configOptions.Value.UserFolderVolumePath}"
        );
        containerInspect.Config.Env.ShouldContain(
            $"ASPNETCORE_HTTP_PORTS={_configOptions.Value.Port}"
        );
    }

    [Fact]
    public async Task Allocate_ShouldMountVolumes_WhenUserFolderIsConfigured()
    {
        // Arrange
        var adapter = new AnalysisInstanceDockerAdapter(
            _configOptions,
            _userFolderOptions,
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
            $"scriptbee-analysis-{instanceId}",
            TestContext.Current.CancellationToken
        );

        containerInspect.HostConfig.Binds.ShouldContain(
            $"{TestUserFolderPath}:{_configOptions.Value.UserFolderVolumePath}"
        );
    }

    [Fact]
    public async Task Allocate_ShouldUseUserFolderHostPath_WhenConfigured()
    {
        // Arrange
        var config = new AnalysisDockerConfig
        {
            DockerSocket = _dockerFixture.DockerClient.Configuration.EndpointBaseUri.ToString(),
            Network = DockerFixture.TestNetworkName,
            UserFolderVolumePath = "/root/.scriptbee",
            UserFolderHostPath = OverrideHostPath,
        };
        var adapter = new AnalysisInstanceDockerAdapter(
            Options.Create(config),
            _userFolderOptions,
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
            $"scriptbee-analysis-{instanceId}",
            TestContext.Current.CancellationToken
        );

        containerInspect.HostConfig.Binds.ShouldContain(
            $"{OverrideHostPath}:{_configOptions.Value.UserFolderVolumePath}"
        );
    }

    [Fact]
    public async Task Allocate_ShouldHaveScriptBeeVolume_WhenNoUserFolderIsConfigured()
    {
        // Arrange
        var adapter = new AnalysisInstanceDockerAdapter(
            _configOptions,
            Options.Create(new UserFolderSettings { UserFolderPath = null }),
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
            $"scriptbee-analysis-{instanceId}",
            TestContext.Current.CancellationToken
        );

        containerInspect.HostConfig.Binds.ShouldContain("scriptbee-plugins:/app/plugins:ro");
    }

    [Fact]
    public async Task Allocate_ShouldUseOverrideMongoDbConnectionString_WhenConfigured()
    {
        // Arrange
        var config = new AnalysisDockerConfig
        {
            DockerSocket = _dockerFixture.DockerClient.Configuration.EndpointBaseUri.ToString(),
            Network = DockerFixture.TestNetworkName,
            UserFolderVolumePath = "/root/.scriptbee",
            MongoDbConnectionString = OverrideMongoConnectionString,
        };
        var adapter = new AnalysisInstanceDockerAdapter(
            Options.Create(config),
            _userFolderOptions,
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
            $"scriptbee-analysis-{instanceId}",
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
        var adapter = new AnalysisInstanceDockerAdapter(
            _configOptions,
            _userFolderOptions,
            _configuration,
            _logger,
            _freePortProvider
        );
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(ProjectId.FromValue("id"));
        var instanceId = new InstanceId(Guid.NewGuid());
        var instanceImage = new AnalysisInstanceImage(DockerFixture.TestImageName);
        var containerName = $"scriptbee-analysis-{instanceId}";

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
            AnalysisInstanceStatus.NotFound
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
        var adapter = new AnalysisInstanceDockerAdapter(
            _configOptions,
            _userFolderOptions,
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
            AnalysisInstanceStatus.NotFound
        );

        var exception = await Record.ExceptionAsync(() =>
            adapter.Deallocate(instanceInfo, TestContext.Current.CancellationToken)
        );

        exception.ShouldBeNull();
    }
}
