using ScriptBee.Domain.Model.File;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class LoadContextService(
    ILoadModelFilesService loadModelFilesService,
    IGenerateClassesUseCase generateClassesUseCase
) : ILoadContextUseCase
{
    public async Task Load(
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken
    )
    {
        await loadModelFilesService.LoadModelFiles(filesToLoad, cancellationToken);

        await generateClassesUseCase.GenerateClasses(cancellationToken);
    }
}
