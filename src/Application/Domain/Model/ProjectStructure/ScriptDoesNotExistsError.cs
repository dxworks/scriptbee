namespace ScriptBee.Domain.Model.ProjectStructure;

public record ScriptDoesNotExistsError(ScriptId ScriptId)
{
    public override string ToString()
    {
        return $"Script '{ScriptId}' does not exist.";
    }
}
