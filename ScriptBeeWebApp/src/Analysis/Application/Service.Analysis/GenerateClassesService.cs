using ScriptBee.Common.CodeGeneration;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class GenerateClassesService(IProjectStructureService projectStructureService)
    : IGenerateClassesUseCase
{
    public Task<IEnumerable<SampleCodeFile>> GenerateClasses(
        List<string> languages,
        CancellationToken cancellationToken
    )
    {
        return projectStructureService.GenerateModelClasses(languages, cancellationToken);
    }
}
