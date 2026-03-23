namespace ScriptBee.Ports.Files;

public record FileDoesNotExistsError(string Path)
{
    public override string ToString() => $"File does not exist: {Path}";
}
