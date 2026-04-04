namespace ScriptBee.Service.Project.ProjectStructure;

public record SampleCodeFile
{
    public required string Name { get; init; }

    public required string Content { get; init; }
}
