namespace ScriptBee.Rest.Contracts;

public class RestContextLoad
{
    public required IDictionary<string, List<string>> FilesToLoad { get; set; }
}
