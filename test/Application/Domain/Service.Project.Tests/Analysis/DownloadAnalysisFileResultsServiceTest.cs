using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Files;
using ScriptBee.Service.Project.Analysis;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class DownloadAnalysisFileResultsServiceTest
{
    private readonly IGetAnalysis _getAnalysis = Substitute.For<IGetAnalysis>();
    private readonly IFileModelService _fileModelService = Substitute.For<IFileModelService>();

    private readonly DownloadAnalysisFileResultsService _downloadAnalysisFileResultsService;

    public DownloadAnalysisFileResultsServiceTest()
    {
        _downloadAnalysisFileResultsService = new DownloadAnalysisFileResultsService(
            _getAnalysis,
            _fileModelService
        );
    }

    [Fact]
    public async Task GivenNoAnalysis_WhenGetFileResultStream_ThenReturnAnalysisDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("id");
        var analysisId = new AnalysisId(Guid.NewGuid());
        var resultId = new ResultId(Guid.NewGuid());
        _getAnalysis
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(
                    new AnalysisDoesNotExistsError(analysisId)
                )
            );

        var result = await _downloadAnalysisFileResultsService.GetFileResultStream(
            projectId,
            analysisId,
            resultId
        );

        result.AsT1.ShouldBe(new AnalysisDoesNotExistsError(analysisId));
    }

    [Fact]
    public async Task GivenNoAnalysisResult_WhenGetFileResultStream_ThenReturnAnalysisResultDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("id");
        var analysisId = new AnalysisId(Guid.NewGuid());
        var resultId = new ResultId(Guid.NewGuid());
        var analysisInfo = CreateAnalysisInfo(
            analysisId,
            projectId,
            [
                new ResultSummary(
                    resultId,
                    "ConsoleOutput",
                    RunResultDefaultTypes.ConsoleType,
                    DateTimeOffset.UtcNow
                ),
            ],
            []
        );
        _getAnalysis
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(analysisInfo)
            );

        var result = await _downloadAnalysisFileResultsService.GetFileResultStream(
            projectId,
            analysisId,
            resultId
        );

        result.AsT2.ShouldBe(new AnalysisResultDoesNotExistsError(resultId));
    }

    [Fact]
    public async Task GivenAnalysisResult_WhenGetFileResultStream_ThenReturnFileStream()
    {
        var projectId = ProjectId.FromValue("id");
        var analysisId = new AnalysisId(Guid.NewGuid());
        var resultId = new ResultId(Guid.NewGuid());
        var analysisInfo = CreateAnalysisInfo(
            analysisId,
            projectId,
            [
                new ResultSummary(
                    resultId,
                    "file.txt",
                    RunResultDefaultTypes.FileType,
                    DateTimeOffset.UtcNow
                ),
                new ResultSummary(
                    resultId,
                    "ConsoleOutput",
                    RunResultDefaultTypes.ConsoleType,
                    DateTimeOffset.UtcNow
                ),
            ],
            []
        );
        var stream = new NamedFileStream(
            "file.txt",
            new MemoryStream("console content"u8.ToArray())
        );
        _getAnalysis
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(analysisInfo)
            );
        _fileModelService
            .GetFileAsync(resultId.ToFileId(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(stream.Stream));

        var result = await _downloadAnalysisFileResultsService.GetFileResultStream(
            projectId,
            analysisId,
            resultId
        );

        result.AsT0.ShouldBe(stream);
    }

    [Fact]
    public async Task GivenNoAnalysis_GetAllFilesZipStream_ThenReturnAnalysisDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("id");
        var analysisId = new AnalysisId(Guid.NewGuid());
        _getAnalysis
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(
                    new AnalysisDoesNotExistsError(analysisId)
                )
            );

        var result = await _downloadAnalysisFileResultsService.GetAllFilesZipStream(
            projectId,
            analysisId
        );

        result.AsT1.ShouldBe(new AnalysisDoesNotExistsError(analysisId));
    }

    [Fact]
    public async Task GivenAnalysisResult_GetAllFilesZipStream_ThenReturnFileStream()
    {
        var projectId = ProjectId.FromValue("id");
        var analysisId = new AnalysisId(Guid.NewGuid());
        var resultId = new ResultId(Guid.NewGuid());
        var analysisInfo = CreateAnalysisInfo(
            analysisId,
            projectId,
            [
                new ResultSummary(
                    resultId,
                    "file.txt",
                    RunResultDefaultTypes.FileType,
                    DateTimeOffset.UtcNow
                ),
                new ResultSummary(
                    resultId,
                    "ConsoleOutput",
                    RunResultDefaultTypes.ConsoleType,
                    DateTimeOffset.UtcNow
                ),
            ],
            []
        );
        _getAnalysis
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(analysisInfo)
            );
        _fileModelService
            .GetFileAsync(resultId.ToFileId(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Stream>(new MemoryStream("console content"u8.ToArray())));

        var result = await _downloadAnalysisFileResultsService.GetAllFilesZipStream(
            projectId,
            analysisId
        );

        result.AsT0.Name.ShouldBe($"{analysisId.Value}.zip");
    }

    private static AnalysisInfo CreateAnalysisInfo(
        AnalysisId analysisId,
        ProjectId projectId,
        IEnumerable<ResultSummary> results,
        IEnumerable<AnalysisError> errors
    )
    {
        return new AnalysisInfo(
            analysisId,
            projectId,
            new ScriptId(Guid.NewGuid()),
            new FileId(Guid.NewGuid()),
            AnalysisStatus.Finished,
            results,
            errors,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow
        );
    }
}
