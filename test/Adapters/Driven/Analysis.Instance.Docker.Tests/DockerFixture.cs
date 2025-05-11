using System.Net;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace ScriptBee.Analysis.Instance.Docker.Tests;

public class DockerFixture : IAsyncLifetime
{
    public const string TestImageName = "mongo:8.0.4";
    public const string TestNetworkName = "integration-test-network";

    public readonly DockerClient DockerClient = new DockerClientConfiguration().CreateClient();

    public async ValueTask InitializeAsync()
    {
        await EnsureNetworkExists();

        await PullTestImageIfNotExists();
    }

    public async ValueTask DisposeAsync()
    {
        var containers = await DockerClient.Containers.ListContainersAsync(
            new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {
                        "name",
                        new Dictionary<string, bool> { { "scriptbee-calculation", true } }
                    },
                },
            }
        );
        foreach (var container in containers)
        {
            try
            {
                await DockerClient.Containers.StopContainerAsync(
                    container.ID,
                    new ContainerStopParameters()
                );
                await DockerClient.Containers.RemoveContainerAsync(
                    container.ID,
                    new ContainerRemoveParameters { Force = true }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cleaning up container {0} error: {1}", container.ID, ex);
            }
        }

        try
        {
            await DockerClient.Networks.DeleteNetworkAsync(TestNetworkName);
        }
        catch (DockerApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Network might not exist, which is fine
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error cleaning up network {0} error: {1}", TestNetworkName, ex);
        }

        DockerClient.Dispose();
    }

    private async Task PullTestImageIfNotExists()
    {
        var images = await DockerClient.Images.ListImagesAsync(
            new ImagesListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {
                        "reference",
                        new Dictionary<string, bool> { { TestImageName, true } }
                    },
                },
            }
        );
        if (!images.Any())
        {
            await DockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = TestImageName.Split(":")[0],
                    Tag = TestImageName.Split(":")[1],
                },
                null,
                new Progress<JSONMessage>()
            );
        }
    }

    private async Task EnsureNetworkExists()
    {
        var networks = await DockerClient.Networks.ListNetworksAsync();
        if (networks.All(n => n.Name != TestNetworkName))
        {
            await DockerClient.Networks.CreateNetworkAsync(
                new NetworksCreateParameters { Name = TestNetworkName }
            );
        }
    }
}
