using Docker.DotNet.Models;

namespace ScriptBee.Analysis.Instance.Docker.Config;

public class AnalysisDockerConfig
{
    public required string DockerSocket { get; init; }

    public int Port { get; init; }

    public string? Network { get; init; }

    public string? MongoDbConnectionString { get; init; }

    public string? UserFolderVolumePath { get; init; }

    public string? UserFolderHostPath { get; init; }

    public string PluginsVolume { get; init; } = null!;

    public HostConfig? HostConfig { get; init; }

    public Dictionary<string, string> Labels { get; init; } = new();
}
