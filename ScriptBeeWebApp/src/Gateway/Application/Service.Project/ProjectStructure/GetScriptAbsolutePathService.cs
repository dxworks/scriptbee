using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

public sealed class GetScriptAbsolutePathService(IConfigFoldersService configFoldersService)
    : IGetScriptAbsolutePathUseCase
{
    public string GetScriptAbsolutePath(Script script)
    {
        return configFoldersService.GetAbsolutePathToSrcFolder(script.ProjectId, script.File.Path);
    }
}
