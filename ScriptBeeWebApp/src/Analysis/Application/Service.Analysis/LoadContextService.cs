using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using ScriptBee.Domain.Model.File;

namespace ScriptBee.Service.Analysis;

public class LoadContextService(ILoadModelFilesService loadModelFilesService) : ILoadContextUseCase
{
    public async Task Load(
        IDictionary<string, IEnumerable<FileId>> filesToLoad,
        CancellationToken cancellationToken
    )
    {
        await loadModelFilesService.LoadModelFiles(filesToLoad, cancellationToken);
    }
}
