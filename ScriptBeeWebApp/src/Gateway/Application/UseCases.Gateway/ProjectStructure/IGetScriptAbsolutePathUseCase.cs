using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Gateway.ProjectStructure;

public interface IGetScriptAbsolutePathUseCase
{
    string GetScriptAbsolutePath(ProjectStructureEntry entry);
}
