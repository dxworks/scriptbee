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
    ILogger<CalculationInstanceDockerAdapter> logger
) : IAllocateInstance, IDeallocateInstance
{
    public async Task<string> Allocate(
        AnalysisInstanceImage image,
        CancellationToken cancellationToken = default
    )
    {
        var calculationDockerConfig = config.Value;
        using var client = new DockerClientConfiguration(
            new Uri(calculationDockerConfig.DockerSocket)
        ).CreateClient();

        await PullImageIfNeeded(client, image.ImageName, cancellationToken);

        var response = await client.Containers.CreateContainerAsync(
            new CreateContainerParameters
            {
                Name = "scriptbee-calculation",
                Image = image.ImageName,
                HostConfig = new HostConfig { NetworkMode = calculationDockerConfig.Network },
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
            calculationDockerConfig.Port,
            cancellationToken
        );
    }

    public Task Deallocate(InstanceInfo calculationInstanceInfo)
    {
        throw new NotImplementedException();
    }

    private static async Task<string> GetContainerUrl(
        DockerClient client,
        string containerId,
        string networkName,
        int port,
        CancellationToken cancellationToken
    )
    {
        var containerInfo = await client.Containers.InspectContainerAsync(
            containerId,
            cancellationToken
        );

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
