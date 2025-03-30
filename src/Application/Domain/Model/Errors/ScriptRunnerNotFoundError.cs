using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Domain.Model.Errors;

public record ScriptRunnerNotFoundError(ScriptLanguage Language)
{
    public override string ToString() => $"Runner for language '{Language.Name}' does not exist.";
}
