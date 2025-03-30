using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScriptBee.Analysis.Instance.Docker.Config;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using Xunit.Abstractions;

namespace ScriptBee.Analysis.Instance.Docker.Tests;

public class CalculationInstanceDockerAdapterTest : IClassFixture<DockerFixture>
{
    private readonly IOptions<CalculationDockerConfig> _config;
    private readonly DockerFixture _dockerFixture;

    private readonly CalculationInstanceDockerAdapter _calculationInstanceDockerAdapter;

    public CalculationInstanceDockerAdapterTest(
        DockerFixture dockerFixture,
        ITestOutputHelper outputHelper
    )
    {
        _dockerFixture = dockerFixture;
        _config = Options.Create(
            new CalculationDockerConfig
            {
                DockerSocket = dockerFixture.DockerClient.Configuration.EndpointBaseUri.ToString(),
                Network = DockerFixture.TestNetworkName,
                Port = 8080,
            }
        );

        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new XUnitLoggerProvider(outputHelper))
        );
        var logger = loggerFactory.CreateLogger<CalculationInstanceDockerAdapter>();

        _calculationInstanceDockerAdapter = new CalculationInstanceDockerAdapter(_config, logger);
    }

    [Fact]
    public async Task Allocate_ShouldCreateAndStartContainerAndReturnUrlWithNetworkIP()
    {
        var instanceId = new InstanceId(Guid.NewGuid());
        var image = new AnalysisInstanceImage(DockerFixture.TestImageName);

        var containerUrl = await _calculationInstanceDockerAdapter.Allocate(instanceId, image);

        containerUrl.ShouldStartWith("http://");
        containerUrl.ShouldContain($":{_config.Value.Port}");
        var containers = await _dockerFixture.DockerClient.Containers.ListContainersAsync(
            new ContainersListParameters { All = true }
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
            $"http://{ourContainer.NetworkSettings.Networks[DockerFixture.TestNetworkName].IPAddress}:{_config.Value.Port}"
        );
    }

    [Fact]
    public async Task Allocate_ShouldUseConfiguredNetworkAndContainerName()
    {
        var instanceId = new InstanceId(Guid.NewGuid());
        var image = new AnalysisInstanceImage(DockerFixture.TestImageName);

        await _calculationInstanceDockerAdapter.Allocate(instanceId, image);

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
            }
        );
        containers.ShouldHaveSingleItem();
        containers.First().Names.ShouldContain($"/scriptbee-calculation-{instanceId}");
        containers.First().NetworkSettings.Networks.ShouldContainKey(DockerFixture.TestNetworkName);
    }

    [Fact]
    public async Task Deallocate_ShouldStopAndRemoveExistingContainer()
    {
        // Arrange
        var instanceId = new InstanceId(Guid.NewGuid());
        var instanceImage = new AnalysisInstanceImage(DockerFixture.TestImageName);
        var containerName = $"scriptbee-calculation-{instanceId}";

        var instanceUrl = await _calculationInstanceDockerAdapter.Allocate(
            instanceId,
            instanceImage
        );
        instanceUrl.ShouldStartWith("http://");
        var instanceInfo = new InstanceInfo(
            instanceId,
            ProjectId.FromValue("project-id"),
            instanceUrl,
            DateTimeOffset.UtcNow
        );

        // Act
        await _calculationInstanceDockerAdapter.Deallocate(instanceInfo);

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
            }
        );
        containers.ShouldBeEmpty();
    }

    [Fact]
    public async Task Deallocate_ShouldNotThrowException_IfContainerNotFound()
    {
        var instanceId = new InstanceId(Guid.NewGuid());
        var instanceInfo = new InstanceInfo(
            instanceId,
            ProjectId.FromValue("project-id"),
            "http://fakeurl",
            DateTimeOffset.UtcNow
        );

        var exception = await Record.ExceptionAsync(
            () => _calculationInstanceDockerAdapter.Deallocate(instanceInfo)
        );

        exception.ShouldBeNull();
    }
}
