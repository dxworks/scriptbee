using System.IO.Compression;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Files;
using ScriptBee.UseCases.Project.Analysis;

namespace ScriptBee.Service.Project.Analysis;

using GetAllFilesZipResultStreamType = OneOf<NamedFileStream, AnalysisDoesNotExistsError>;
using GetFileResultStreamType = OneOf<
    NamedFileStream,
    AnalysisDoesNotExistsError,
    AnalysisResultDoesNotExistsError
>;

public class DownloadAnalysisFileResultsService(
    IGetAnalysis getAnalysis,
    IFileModelService fileModelService
) : IDownloadAnalysisFileResultsUseCase
{
    public async Task<GetFileResultStreamType> GetFileResultStream(
        ProjectId projectId,
        AnalysisId analysisId,
        ResultId resultId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);

        return await result.Match<Task<GetFileResultStreamType>>(
            async analysisInfo =>
                await GetFileResultStream(analysisInfo, resultId, cancellationToken),
            error => Task.FromResult<GetFileResultStreamType>(error)
        );
    }

    public async Task<GetAllFilesZipResultStreamType> GetAllFilesZipStream(
        ProjectId projectId,
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);

        return await result.Match<Task<GetAllFilesZipResultStreamType>>(
            async analysisInfo => await GetAllFilesZipStream(analysisInfo, cancellationToken),
            error => Task.FromResult<GetAllFilesZipResultStreamType>(error)
        );
    }

    private async Task<GetFileResultStreamType> GetFileResultStream(
        AnalysisInfo analysisInfo,
        ResultId resultId,
        CancellationToken cancellationToken
    )
    {
        var result = analysisInfo.Results.FirstOrDefault(r =>
            r.Type == RunResultDefaultTypes.FileType && r.Id == resultId
        );

        if (result == null)
        {
            return new AnalysisResultDoesNotExistsError(resultId);
        }

        var stream = await fileModelService.GetFileAsync(resultId.ToFileId(), cancellationToken);
        return new NamedFileStream(result.Name, stream);
    }

    private async Task<GetAllFilesZipResultStreamType> GetAllFilesZipStream(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken
    )
    {
        var results = analysisInfo.Results.Where(r => r.Type == RunResultDefaultTypes.FileType);

        var fileNamedStreams = new List<NamedFileStream>();

        foreach (var result in results)
        {
            var stream = await fileModelService.GetFileAsync(
                result.Id.ToFileId(),
                cancellationToken
            );
            fileNamedStreams.Add(new NamedFileStream(result.Name, stream));
        }

        return new NamedFileStream(
            $"{analysisInfo.Id}.zip",
            CreateFilesZipStream(fileNamedStreams)
        );
    }

    private static MemoryStream CreateFilesZipStream(IEnumerable<NamedFileStream> outputFiles)
    {
        var zipStream = new MemoryStream();
        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var (name, content) in outputFiles)
            {
                var zipArchiveEntry = zip.CreateEntry(name);

                using var destinationStream = zipArchiveEntry.Open();

                var buffer = new byte[1024];
                int len;
                while ((len = content.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destinationStream.Write(buffer, 0, len);
                }
            }
        }

        zipStream.Position = 0;
        return zipStream;
    }
}
