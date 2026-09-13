using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

namespace ScriptBee.Service.Analysis;

public class ClearContextService(IProjectManager projectManager) : IClearContextUseCase
{
    public void Clear()
    {
        var project = projectManager.GetProject();

        project.Context.Clear();
    }
}
