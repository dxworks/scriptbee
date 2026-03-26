namespace ScriptBee.Artifacts;

public record FileDoesNotExistsError(string Path)
{
    public override string ToString() => $"File does not exist: {Path}";
}

