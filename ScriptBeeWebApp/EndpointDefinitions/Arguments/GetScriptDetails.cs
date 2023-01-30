namespace ScriptBeeWebApp.EndpointDefinitions.Arguments;

// todo: replace with AsParameters when upgrading to .NET 7
public class GetScriptDetails
{
    public string ProjectId { get; init; } = null!;
    public string FilePath { get; init; } = null!;
}
