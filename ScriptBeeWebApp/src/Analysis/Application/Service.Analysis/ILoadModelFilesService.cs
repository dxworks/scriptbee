using ScriptBee.Domain.Model.File;

namespace ScriptBee.Service.Analysis;

public interface ILoadModelFilesService
{
    Task LoadModelFiles(
        IDictionary<string, IEnumerable<FileId>> loadedFiles,
        CancellationToken cancellationToken = default
    );
}
