using System.Net;
using k8s;
using k8s.Autorest;
using k8s.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScriptBee.Analysis.Instance.Kubernetes.Config;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance.Allocation;

namespace ScriptBee.Analysis.Instance.Kubernetes;

public sealed class AnalysisInstanceKubernetesAdapter(
    IKubernetesClientFactory clientFactory,
    IOptions<AnalysisKubernetesConfig> options,
    IConfiguration configuration,
    ILogger<AnalysisInstanceKubernetesAdapter> logger
) : IAllocateInstance, IDeallocateInstance, IGetInstanceStatus
{
    public async Task<string> Allocate(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        AnalysisInstanceImage image,
        CancellationToken cancellationToken = default
    )
    {
        var config = options.Value;
        using var client = clientFactory.CreateClient();

        var resourceName = GetResourceName(instanceId);
        var labels = BuildLabels(projectDetails, instanceId, config);
        var pod = BuildPod(projectDetails, instanceId, image, resourceName, labels, config);
        var service = BuildService(resourceName, instanceId, labels, config);

        logger.LogInformation(
            "Creating Kubernetes Pod {PodName} in namespace {Namespace}",
            resourceName,
            config.Namespace
        );
        await client.CoreV1.CreateNamespacedPodAsync(
            pod,
            config.Namespace,
            cancellationToken: cancellationToken
        );

        logger.LogInformation(
            "Creating Kubernetes Service {ServiceName} in namespace {Namespace}",
            resourceName,
            config.Namespace
        );
        await client.CoreV1.CreateNamespacedServiceAsync(
            service,
            config.Namespace,
            cancellationToken: cancellationToken
        );

        return $"http://{resourceName}.{config.Namespace}.svc.cluster.local:{config.Port}";
    }

    public async Task Deallocate(InstanceInfo instanceInfo, CancellationToken cancellationToken)
    {
        var config = options.Value;
        using var client = clientFactory.CreateClient();
        var resourceName = GetResourceName(instanceInfo.Id);

        logger.LogInformation(
            "Deallocating Kubernetes resources for instance {ResourceName}",
            resourceName
        );

        await DeletePod(client, resourceName, config.Namespace, cancellationToken);
        await DeleteService(client, resourceName, config.Namespace, cancellationToken);
    }

    public async Task<AnalysisInstanceStatus> GetStatus(
        InstanceId instanceId,
        CancellationToken cancellationToken
    )
    {
        var config = options.Value;
        using var client = clientFactory.CreateClient();
        var resourceName = GetResourceName(instanceId);

        try
        {
            var pod = await client.CoreV1.ReadNamespacedPodStatusAsync(
                resourceName,
                config.Namespace,
                cancellationToken: cancellationToken
            );

            if (pod == null)
            {
                return AnalysisInstanceStatus.NotFound;
            }

            if (pod.Metadata?.DeletionTimestamp != null)
            {
                return AnalysisInstanceStatus.Deallocating;
            }

            var phase = pod.Status?.Phase?.ToLowerInvariant();
            return phase switch
            {
                "pending" => AnalysisInstanceStatus.Allocating,
                "running" => IsPodReady(pod)
                    ? AnalysisInstanceStatus.Running
                    : AnalysisInstanceStatus.Allocating,
                _ => AnalysisInstanceStatus.NotFound,
            };
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
        {
            return AnalysisInstanceStatus.NotFound;
        }
    }

    private static string GetResourceName(InstanceId instanceId)
    {
        return $"scriptbee-analysis-{instanceId.Value.ToString().ToLowerInvariant()}";
    }

    private static Dictionary<string, string> BuildLabels(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        AnalysisKubernetesConfig config
    )
    {
        var labels = new Dictionary<string, string>(config.Labels)
        {
            ["app.kubernetes.io/name"] = "scriptbee-analysis",
            ["scriptbee.org/instance-id"] = instanceId.Value.ToString().ToLowerInvariant(),
            ["scriptbee.org/project-id"] = projectDetails.Id.Value,
        };

        return labels;
    }

    private V1Pod BuildPod(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        AnalysisInstanceImage image,
        string resourceName,
        Dictionary<string, string> labels,
        AnalysisKubernetesConfig config
    )
    {
        var container = new V1Container
        {
            Name = "analysis",
            Image = image.ImageName,
            Ports = new List<V1ContainerPort>
            {
                new() { ContainerPort = config.Port, Name = "http" },
            },
            Env = BuildEnvironmentVariables(projectDetails, instanceId, config),
            VolumeMounts = BuildVolumeMounts(config),
            Resources = BuildResources(config.Resources),
            ReadinessProbe = new V1Probe
            {
                HttpGet = new V1HTTPGetAction { Port = config.Port, Path = "/health" },
                InitialDelaySeconds = 2,
                PeriodSeconds = 2,
                TimeoutSeconds = 2,
                FailureThreshold = 3,
            },
        };

        return new V1Pod
        {
            Metadata = new V1ObjectMeta
            {
                Name = resourceName,
                NamespaceProperty = config.Namespace,
                Labels = labels,
            },
            Spec = new V1PodSpec
            {
                RestartPolicy = "Never",
                Containers = new List<V1Container> { container },
                Volumes = BuildVolumes(config),
                NodeSelector = config.NodeSelector,
                Tolerations = BuildTolerations(config.Tolerations),
            },
        };
    }

    private static V1Service BuildService(
        string resourceName,
        InstanceId instanceId,
        Dictionary<string, string> labels,
        AnalysisKubernetesConfig config
    )
    {
        return new V1Service
        {
            Metadata = new V1ObjectMeta
            {
                Name = resourceName,
                NamespaceProperty = config.Namespace,
                Labels = labels,
            },
            Spec = new V1ServiceSpec
            {
                Type = "ClusterIP",
                Selector = new Dictionary<string, string>
                {
                    ["scriptbee.org/instance-id"] = instanceId.Value.ToString().ToLowerInvariant(),
                },
                Ports = new List<V1ServicePort>
                {
                    new()
                    {
                        Port = config.Port,
                        Name = "http",
                        TargetPort = config.Port,
                    },
                },
            },
        };
    }

    private List<V1EnvVar> BuildEnvironmentVariables(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        AnalysisKubernetesConfig config
    )
    {
        var mongoConnectionString =
            config.MongoDbConnectionString ?? configuration.GetConnectionString("mongodb");

        var envVars = new List<V1EnvVar>
        {
            new() { Name = "ScriptBee__InstanceId", Value = instanceId.Value.ToString() },
            new() { Name = "ScriptBee__ProjectId", Value = projectDetails.Id.Value },
            new() { Name = "ScriptBee__ProjectName", Value = projectDetails.Name },
            new() { Name = "ConnectionStrings__mongodb", Value = mongoConnectionString },
            new() { Name = "ASPNETCORE_HTTP_PORTS", Value = config.Port.ToString() },
        };

        if (!string.IsNullOrWhiteSpace(config.UserFolderVolumePath))
        {
            envVars.Add(
                new V1EnvVar
                {
                    Name = "UserFolder__UserFolderPath",
                    Value = config.UserFolderVolumePath,
                }
            );
        }

        var telemetrySection = configuration.GetSection("ScriptBee:Telemetry");
        if (bool.TryParse(telemetrySection["Enabled"], out var enabled))
        {
            envVars.Add(
                new V1EnvVar { Name = "ScriptBee__Telemetry__Enabled", Value = enabled.ToString() }
            );
        }

        var otlpEndpoint = telemetrySection["OtlpEndpoint"];
        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            envVars.Add(
                new V1EnvVar { Name = "ScriptBee__Telemetry__OtlpEndpoint", Value = otlpEndpoint }
            );
        }

        if (
            bool.TryParse(
                telemetrySection["ConsoleExporterEnabled"],
                out var consoleExporterEnabled
            )
        )
        {
            envVars.Add(
                new V1EnvVar
                {
                    Name = "ScriptBee__Telemetry__ConsoleExporterEnabled",
                    Value = consoleExporterEnabled.ToString(),
                }
            );
        }

        return envVars;
    }

    private static List<V1VolumeMount> BuildVolumeMounts(AnalysisKubernetesConfig config)
    {
        var mounts = new List<V1VolumeMount>();

        if (
            !string.IsNullOrWhiteSpace(config.UserFolderPvcName)
            && !string.IsNullOrWhiteSpace(config.UserFolderVolumePath)
        )
        {
            mounts.Add(
                new V1VolumeMount { MountPath = config.UserFolderVolumePath, Name = "user-folder" }
            );
        }

        if (!string.IsNullOrWhiteSpace(config.PluginsPvcName))
        {
            mounts.Add(
                new V1VolumeMount
                {
                    MountPath = "/app/plugins",
                    Name = "plugins-volume",
                    ReadOnlyProperty = true,
                }
            );
        }

        return mounts;
    }

    private static List<V1Volume> BuildVolumes(AnalysisKubernetesConfig config)
    {
        var volumes = new List<V1Volume>();

        if (
            !string.IsNullOrWhiteSpace(config.UserFolderPvcName)
            && !string.IsNullOrWhiteSpace(config.UserFolderVolumePath)
        )
        {
            volumes.Add(
                new V1Volume
                {
                    Name = "user-folder",
                    PersistentVolumeClaim = new V1PersistentVolumeClaimVolumeSource
                    {
                        ClaimName = config.UserFolderPvcName,
                    },
                }
            );
        }

        if (!string.IsNullOrWhiteSpace(config.PluginsPvcName))
        {
            volumes.Add(
                new V1Volume
                {
                    Name = "plugins-volume",
                    PersistentVolumeClaim = new V1PersistentVolumeClaimVolumeSource
                    {
                        ClaimName = config.PluginsPvcName,
                    },
                }
            );
        }

        return volumes;
    }

    private static V1ResourceRequirements? BuildResources(PodResourceRequirements? resources)
    {
        if (resources == null)
        {
            return null;
        }

        var requirements = new V1ResourceRequirements();

        if (resources.Requests != null && resources.Requests.Count > 0)
        {
            requirements.Requests = resources.Requests.ToDictionary(
                pair => pair.Key,
                pair => new ResourceQuantity(pair.Value)
            );
        }

        if (resources.Limits != null && resources.Limits.Count > 0)
        {
            requirements.Limits = resources.Limits.ToDictionary(
                pair => pair.Key,
                pair => new ResourceQuantity(pair.Value)
            );
        }

        return requirements;
    }

    private static List<V1Toleration>? BuildTolerations(List<PodTolerationConfig>? tolerations)
    {
        if (tolerations == null || tolerations.Count == 0)
        {
            return null;
        }

        return tolerations
            .Select(t => new V1Toleration
            {
                Key = t.Key,
                OperatorProperty = t.Operator,
                Value = t.Value,
                Effect = t.Effect,
                TolerationSeconds = t.TolerationSeconds,
            })
            .ToList();
    }

    private async Task DeletePod(
        IKubernetes client,
        string resourceName,
        string ns,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await client.CoreV1.DeleteNamespacedPodAsync(
                resourceName,
                ns,
                cancellationToken: cancellationToken
            );
            logger.LogInformation(
                "Deleted Kubernetes Pod {PodName} in namespace {Namespace}",
                resourceName,
                ns
            );
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning(
                "Pod {PodName} in namespace {Namespace} was not found for deletion",
                resourceName,
                ns
            );
        }
    }

    private async Task DeleteService(
        IKubernetes client,
        string resourceName,
        string ns,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await client.CoreV1.DeleteNamespacedServiceAsync(
                resourceName,
                ns,
                cancellationToken: cancellationToken
            );
            logger.LogInformation(
                "Deleted Kubernetes Service {ServiceName} in namespace {Namespace}",
                resourceName,
                ns
            );
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning(
                "Service {ServiceName} in namespace {Namespace} was not found for deletion",
                resourceName,
                ns
            );
        }
    }

    private static bool IsPodReady(V1Pod pod)
    {
        return pod.Status?.Conditions?.Any(c =>
                string.Equals(c.Type, "Ready", StringComparison.OrdinalIgnoreCase)
                && string.Equals(c.Status, "True", StringComparison.OrdinalIgnoreCase)
            ) == true;
    }
}
