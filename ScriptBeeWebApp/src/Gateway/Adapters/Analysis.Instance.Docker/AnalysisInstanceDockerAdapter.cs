using System.Net;
using System.Text.Json;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScriptBee.Analysis.Instance.Docker.Config;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance.Allocation;

namespace ScriptBee.Analysis.Instance.Docker;

public class AnalysisInstanceDockerAdapter(
    IOptions<AnalysisDockerConfig> config,
    IConfiguration configuration,
    ILogger<AnalysisInstanceDockerAdapter> logger,
    IFreePortProvider freePortProvider
) : IAllocateInstance, IDeallocateInstance, IGetInstanceStatus
{
    public async Task<string> Allocate(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        AnalysisInstanceImage image,
        CancellationToken cancellationToken = default
    )
    {
        var analysisDockerConfig = config.Value;
        using var client = CreateDockerClient(analysisDockerConfig);

        await PullImageIfNeeded(client, image.ImageName, cancellationToken);

        await EnsurePluginVolumeExists(
            client,
            analysisDockerConfig.PluginsVolume,
            cancellationToken
        );

        var hostPort = freePortProvider.GetFreeTcpPort();

        var mongoDbConnectionString =
            analysisDockerConfig.MongoDbConnectionString
            ?? configuration.GetConnectionString("mongodb");

        var response = await client.Containers.CreateContainerAsync(
            new CreateContainerParameters
            {
                Name = $"scriptbee-analysis-{instanceId}",
                Image = image.ImageName,
                HostConfig = GetComputedHostConfig(analysisDockerConfig, hostPort),
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { $"{analysisDockerConfig.Port}/tcp", new EmptyStruct() },
                },
                Env = GetEnvironmentVariables(projectDetails, instanceId, mongoDbConnectionString),
                Volumes = GetVolumes(),
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
            analysisDockerConfig.Network,
            analysisDockerConfig.Port,
            hostPort,
            cancellationToken
        );
    }

    public async Task Deallocate(InstanceInfo instanceInfo, CancellationToken cancellationToken)
    {
        var containerName = $"scriptbee-analysis-{instanceInfo.Id}";
        logger.LogInformation("Attempting to deallocate container: {Name}", containerName);

        var analysisDockerConfig = config.Value;
        using var client = CreateDockerClient(analysisDockerConfig);

        var containers = await client.Containers.ListContainersAsync(
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
            cancellationToken
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
                    new ContainerStopParameters(),
                    cancellationToken
                );

                logger.LogInformation(
                    "Removing container: {Id} - {Name}",
                    container.ID,
                    containerName
                );
                await client.Containers.RemoveContainerAsync(
                    container.ID,
                    new ContainerRemoveParameters { Force = true },
                    cancellationToken
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

    public async Task<AnalysisInstanceStatus> GetStatus(
        InstanceId instanceId,
        CancellationToken cancellationToken
    )
    {
        var containerName = $"scriptbee-analysis-{instanceId}";
        var analysisDockerConfig = config.Value;
        using var client = CreateDockerClient(analysisDockerConfig);

        var containers = await client.Containers.ListContainersAsync(
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
            cancellationToken
        );

        var container = containers.FirstOrDefault();

        if (container == null)
        {
            return AnalysisInstanceStatus.NotFound;
        }

        return container.State.ToLower() switch
        {
            "running" => AnalysisInstanceStatus.Running,
            "created" or "restarting" => AnalysisInstanceStatus.Allocating,
            "removing" => AnalysisInstanceStatus.Deallocating,
            _ => AnalysisInstanceStatus.NotFound,
        };
    }

    private static HostConfig GetComputedHostConfig(
        AnalysisDockerConfig analysisDockerConfig,
        int hostPort
    )
    {
        var baseConfig = analysisDockerConfig.HostConfig ?? new HostConfig();

        var json = JsonSerializer.Serialize(baseConfig);
        var hostConfig = JsonSerializer.Deserialize<HostConfig>(json)!;

        var portBindings = new Dictionary<string, IList<PortBinding>>
        {
            {
                $"{analysisDockerConfig.Port}/tcp",
                new List<PortBinding> { new() { HostPort = hostPort.ToString() } }
            },
        };

        hostConfig.NetworkMode = string.IsNullOrEmpty(analysisDockerConfig.Network)
            ? "bridge"
            : analysisDockerConfig.Network;
        hostConfig.PortBindings = portBindings;

        hostConfig.Binds = GetBinds(analysisDockerConfig);

        return hostConfig;
    }

    private List<string> GetEnvironmentVariables(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        string? mongoDbConnectionString
    )
    {
        var environmentVariables = new List<string>
        {
            $"ScriptBee__InstanceId={instanceId}",
            $"ScriptBee__ProjectId={projectDetails.Id}",
            $"ScriptBee__ProjectName={projectDetails.Name}",
            $"ConnectionStrings__mongodb={mongoDbConnectionString}",
            $"ASPNETCORE_HTTP_PORTS={config.Value.Port}",
        };

        if (!string.IsNullOrEmpty(config.Value.UserFolderVolumePath))
        {
            environmentVariables.Add(
                $"UserFolder__UserFolderPath={config.Value.UserFolderVolumePath}"
            );
        }

        return environmentVariables;
    }

    private Dictionary<string, EmptyStruct> GetVolumes()
    {
        var hostPath = config.Value.UserFolderHostPath;
        if (
            string.IsNullOrEmpty(hostPath)
            || string.IsNullOrEmpty(config.Value.UserFolderVolumePath)
        )
        {
            return new Dictionary<string, EmptyStruct>();
        }

        return new Dictionary<string, EmptyStruct>
        {
            { config.Value.UserFolderVolumePath, new EmptyStruct() },
        };
    }

    private static List<string> GetBinds(AnalysisDockerConfig analysisDockerConfig)
    {
        var binds = new List<string>();

        var userHostPath = analysisDockerConfig.UserFolderHostPath;
        if (!string.IsNullOrEmpty(userHostPath))
        {
            binds.Add($"{userHostPath}:{analysisDockerConfig.UserFolderVolumePath}");
        }

        binds.Add($"{analysisDockerConfig.PluginsVolume}:/app/plugins:ro");

        return binds;
    }

    private static DockerClient CreateDockerClient(AnalysisDockerConfig analysisDockerConfig)
    {
        return new DockerClientConfiguration(
            new Uri(analysisDockerConfig.DockerSocket)
        ).CreateClient();
    }

    private async Task EnsurePluginVolumeExists(
        DockerClient client,
        string pluginsVolume,
        CancellationToken cancellationToken
    )
    {
        var volumes = await client.Volumes.ListAsync(cancellationToken);
        if (volumes.Volumes == null || volumes.Volumes.All(v => v.Name != pluginsVolume))
        {
            logger.LogInformation("Creating named volume: {PluginsVolume}", pluginsVolume);
            await client.Volumes.CreateAsync(
                new VolumesCreateParameters { Name = pluginsVolume },
                cancellationToken
            );
        }
    }

    private static async Task<string> GetContainerUrl(
        DockerClient client,
        string containerId,
        string? networkName,
        int internalPort,
        int hostPort,
        CancellationToken cancellationToken
    )
    {
        var containerInfo = await client.Containers.InspectContainerAsync(
            containerId,
            cancellationToken
        );

        if (
            !string.IsNullOrEmpty(networkName)
            && containerInfo.NetworkSettings.Networks.TryGetValue(networkName, out var network)
        )
        {
            if (!string.IsNullOrEmpty(network.IPAddress))
            {
                return $"http://{network.IPAddress}:{internalPort}";
            }
        }

        return $"http://localhost:{hostPort}";
    }

    private async Task PullImageIfNeeded(
        DockerClient client,
        string imageName,
        CancellationToken cancellationToken
    )
    {
        var parts = imageName.Split(':');
        var image = parts[0];
        var tag = parts.Length > 1 ? parts[1] : "latest";

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
