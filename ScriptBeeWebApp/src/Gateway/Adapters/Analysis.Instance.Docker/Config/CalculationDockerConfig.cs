namespace ScriptBee.Analysis.Instance.Docker.Config;

public class CalculationDockerConfig
{
    public required string DockerSocket { get; init; }

    public int Port { get; init; } = 8080;

    public string? Network { get; init; }

    public string? MongoDbConnectionString { get; init; }

    public string? UserFolderVolumePath { get; init; }

    public string? UserFolderHostPath { get; init; }
}
