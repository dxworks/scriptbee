using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Project.ProjectStructure;

public interface IGetScriptAbsolutePathUseCase
{
    string GetScriptAbsolutePath(ProjectStructureEntry entry);
}
