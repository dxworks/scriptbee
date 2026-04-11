using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class GenerateClassesService(IProjectStructureService projectStructureService)
    : IGenerateClassesUseCase
{
    public Task GenerateClasses(CancellationToken cancellationToken)
    {
        return projectStructureService.GenerateModelClasses(cancellationToken);
    }
}
