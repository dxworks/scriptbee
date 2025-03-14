using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Domain.Model.Plugin;

public record ScriptRunnerNotFoundError(ScriptLanguage Language)
{
    public override string ToString()
    {
        return $"Runner for language '{Language.Name}' does not exist.";
    }
}
