using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using ScriptBee.Common.CodeGeneration;

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
