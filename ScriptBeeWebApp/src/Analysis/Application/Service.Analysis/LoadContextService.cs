using ScriptBee.Domain.Model.File;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class LoadContextService(
    ILoadModelFilesService loadModelFilesService,
    IProjectStructureService projectStructureService
) : ILoadContextUseCase
{
    public async Task Load(
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken
    )
    {
        await loadModelFilesService.LoadModelFiles(filesToLoad, cancellationToken);

        await projectStructureService.GenerateModelClasses(cancellationToken);
    }
}
