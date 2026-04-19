using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

public sealed class GetScriptAbsolutePathService(IConfigFoldersService configFoldersService)
    : IGetScriptAbsolutePathUseCase
{
    public string GetScriptAbsolutePath(ProjectStructureEntry entry)
    {
        return configFoldersService.GetAbsolutePathToSrcFolder(entry.ProjectId, entry.File.Path);
    }
}
