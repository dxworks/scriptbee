using System.Net;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScriptBee.Analysis.Instance.Docker.Config;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Ports.Instance;

namespace ScriptBee.Analysis.Instance.Docker;

public class CalculationInstanceDockerAdapter(
    IOptions<CalculationDockerConfig> config,
    ILogger<CalculationInstanceDockerAdapter> logger,
    IFreePortProvider freePortProvider
) : IAllocateInstance, IDeallocateInstance
{
    public async Task<string> Allocate(
        InstanceId instanceId,
        AnalysisInstanceImage image,
        CancellationToken cancellationToken = default
    )
    {
        var calculationDockerConfig = config.Value;
        using var client = CreateDockerClient(calculationDockerConfig);

        await PullImageIfNeeded(client, image.ImageName, cancellationToken);

        var hostPort = freePortProvider.GetFreeTcpPort();

        var portBindings = new Dictionary<string, IList<PortBinding>>
        {
            {
                $"{calculationDockerConfig.Port}/tcp",
                new List<PortBinding> { new() { HostPort = hostPort.ToString() } }
            },
        };

        var response = await client.Containers.CreateContainerAsync(
            new CreateContainerParameters
            {
                Name = $"scriptbee-calculation-{instanceId}",
                Image = image.ImageName,
                HostConfig = new HostConfig
                {
                    NetworkMode = calculationDockerConfig.Network,
                    PortBindings = portBindings,
                },
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { $"{calculationDockerConfig.Port}/tcp", new EmptyStruct() },
                },
            },
            cancellationToken
        );

        logger.LogInformation("Container Created: {ID}", response.ID);

        await client.Containers.StartContainerAsync(
            response.ID,
            new ContainerStartParameters(),
            cancellationToken
        );
        logger.LogInformation("Container started successfully");

        return await GetContainerUrl(
            client,
            response.ID,
            calculationDockerConfig.Network,
            hostPort,
            cancellationToken
        );
    }

    private static DockerClient CreateDockerClient(CalculationDockerConfig calculationDockerConfig)
    {
        return new DockerClientConfiguration(
            new Uri(calculationDockerConfig.DockerSocket)
        ).CreateClient();
    }

    public async Task Deallocate(InstanceInfo calculationInstanceInfo)
    {
        var containerName = $"scriptbee-calculation-{calculationInstanceInfo.Id}";
        logger.LogInformation("Attempting to deallocate container: {Name}", containerName);

        var calculationDockerConfig = config.Value;
        using var client = CreateDockerClient(calculationDockerConfig);

        IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
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

        var container = containers.FirstOrDefault();
        if (container != null)
        {
            try
            {
                logger.LogInformation(
                    "Stopping container: {Id} - {Name}",
                    container.ID,
                    containerName
                );
                await client.Containers.StopContainerAsync(
                    container.ID,
                    new ContainerStopParameters()
                );

                logger.LogInformation(
                    "Removing container: {Id} - {Name}",
                    container.ID,
                    containerName
                );
                await client.Containers.RemoveContainerAsync(
                    container.ID,
                    new ContainerRemoveParameters { Force = true }
                );

                logger.LogInformation("Container deallocated successfully: {Name}", containerName);
            }
            catch (DockerApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogWarning("Container not found during deallocation: {Name}", containerName);
            }
        }
        else
        {
            logger.LogWarning(
                "No container found with name: {Name} for deallocation",
                containerName
            );
        }
    }

    private static async Task<string> GetContainerUrl(
        DockerClient client,
        string containerId,
        string? networkName,
        int port,
        CancellationToken cancellationToken
    )
    {
        var containerInfo = await client.Containers.InspectContainerAsync(
            containerId,
            cancellationToken
        );

        if (networkName == null)
        {
            return $"http://localhost:{port}";
        }

        return containerInfo.NetworkSettings.Networks.TryGetValue(networkName, out var network)
            ? $"http://{network.IPAddress}:{port}"
            : $"http://localhost:{port}";
    }

    private async Task PullImageIfNeeded(
        DockerClient client,
        string imageName,
        CancellationToken cancellationToken = default
    )
    {
        var parts = imageName.Split(":");
        var image = parts[0];
        var tag = parts[1];

        var images = await client.Images.ListImagesAsync(
            new ImagesListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {
                        "reference",
                        new Dictionary<string, bool> { { $"{image}:{tag}", true } }
                    },
                },
            },
            cancellationToken
        );

        if (images.Count == 0)
        {
            logger.LogInformation("Pulling image {Image}:{Tag}...", image, tag);
            await client.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = image, Tag = tag },
                new AuthConfig(),
                new Progress<JSONMessage>(msg =>
                    logger.LogInformation("Pulling image status {Status}", msg.Status)
                ),
                cancellationToken
            );
        }
        else
        {
            logger.LogInformation("Image {Image}:{Tag} already exists", image, tag);
        }
    }
}
