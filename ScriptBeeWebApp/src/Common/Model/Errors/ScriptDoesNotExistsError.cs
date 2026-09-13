using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Domain.Model.Errors;

public sealed class ScriptDoesNotExistsError
{
    public ScriptId? ScriptId { get; }
    private string? Path { get; }

    public ScriptDoesNotExistsError(ScriptId scriptId)
    {
        ScriptId = scriptId;
    }

    public ScriptDoesNotExistsError(string path)
    {
        Path = path;
    }

    public override string ToString()
    {
        return Path is not null
            ? $"Script at path '{Path}' does not exist."
            : $"Script '{ScriptId}' does not exist.";
    }
}
