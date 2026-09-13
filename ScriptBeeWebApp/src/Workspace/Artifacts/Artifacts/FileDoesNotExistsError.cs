namespace ScriptBee.Artifacts;

public sealed record FileDoesNotExistsError(string Path)
{
    public override string ToString() => $"File does not exist: {Path}";
}
