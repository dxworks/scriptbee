using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class ClearContextService(IProjectManager projectManager) : IClearContextUseCase
{
    public void Clear()
    {
        var project = projectManager.GetProject();

        project.Context.Clear();
    }
}
