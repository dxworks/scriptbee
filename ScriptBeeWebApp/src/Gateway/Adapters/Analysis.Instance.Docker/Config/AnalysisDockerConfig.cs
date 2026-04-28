using Docker.DotNet.Models;

namespace ScriptBee.Analysis.Instance.Docker.Config;

public class AnalysisDockerConfig
{
    public required string DockerSocket { get; init; }

    public int Port { get; init; } = 80;

    public string? Network { get; init; }

    public string? MongoDbConnectionString { get; init; }

    public string? UserFolderVolumePath { get; init; }

    public string? UserFolderHostPath { get; init; }

    public string PluginsVolume { get; init; } = "scriptbee-plugins";

    public HostConfig? HostConfig { get; init; }
}
