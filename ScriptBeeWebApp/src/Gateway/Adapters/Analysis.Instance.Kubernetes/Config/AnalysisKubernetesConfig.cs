namespace ScriptBee.Analysis.Instance.Kubernetes.Config;

public sealed class AnalysisKubernetesConfig
{
    public string Namespace { get; init; } = "default";

    public string? KubeConfigPath { get; init; }

    public int Port { get; init; } = 80;

    public string? MongoDbConnectionString { get; init; }

    public string? UserFolderVolumePath { get; init; }

    public string? UserFolderPvcName { get; init; }

    public string? PluginsPvcName { get; init; }

    public Dictionary<string, string> Labels { get; init; } = new();

    public PodResourceRequirements? Resources { get; init; }

    public Dictionary<string, string>? NodeSelector { get; init; }

    public List<PodTolerationConfig>? Tolerations { get; init; }
}

public sealed class PodResourceRequirements
{
    public Dictionary<string, string>? Requests { get; init; }

    public Dictionary<string, string>? Limits { get; init; }
}

public sealed class PodTolerationConfig
{
    public string? Key { get; init; }

    public string? Operator { get; init; }

    public string? Value { get; init; }

    public string? Effect { get; init; }

    public long? TolerationSeconds { get; init; }
}
