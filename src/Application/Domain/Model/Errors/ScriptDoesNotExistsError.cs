using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Domain.Model.Errors;

public record ScriptDoesNotExistsError(ScriptId ScriptId)
{
    public override string ToString() => $"Script '{ScriptId}' does not exist.";
}
