using ScriptBee.Domain.Model.File;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class LoadContextService(ILoadModelFilesService loadModelFilesService) : ILoadContextUseCase
{
    public async Task Load(
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken
    )
    {
        await loadModelFilesService.LoadModelFiles(filesToLoad, cancellationToken);

        // TODO FIXIT(#54): generate model classes
        // await projectStructureService.GenerateModelClasses(loadModels.ProjectId, cancellationToken);
    }
}
