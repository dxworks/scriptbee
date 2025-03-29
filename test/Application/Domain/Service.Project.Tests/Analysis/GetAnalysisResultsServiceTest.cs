using DxWorks.ScriptBee.Plugin.Api.Services;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Files;
using ScriptBee.Service.Project.Analysis;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class GetAnalysisResultsServiceTest
{
    private readonly IGetAnalysis _getAnalysis = Substitute.For<IGetAnalysis>();
    private readonly IFileModelService _fileModelService = Substitute.For<IFileModelService>();

    private readonly GetAnalysisResultsService _getAnalysisResultsService;

    public GetAnalysisResultsServiceTest()
    {
        _getAnalysisResultsService = new GetAnalysisResultsService(_getAnalysis, _fileModelService);
    }

    [Fact]
    public async Task GivenNoAnalysis_WhenGetConsoleResult_ThenReturnAnalysisDoesNotExistsError()
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

        var result = await _getAnalysisResultsService.GetConsoleResult(projectId, analysisId);

        result.AsT1.ShouldBe(new AnalysisDoesNotExistsError(analysisId));
    }

    [Fact]
    public async Task GivenAnalysis_WhenGetConsoleResult_ThenReturnConsoleContent()
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
        _fileModelService
            .GetFileAsync(resultId.ToFileId(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Stream>(new MemoryStream("console content"u8.ToArray())));

        var result = await _getAnalysisResultsService.GetConsoleResult(projectId, analysisId);

        result.AsT0.ShouldBe("console content");
    }

    [Fact]
    public async Task GivenAnalysisWithoutConsoleResult_WhenGetConsoleResult_ThenReturnEmptyConsoleContent()
    {
        var projectId = ProjectId.FromValue("id");
        var analysisId = new AnalysisId(Guid.NewGuid());
        var analysisInfo = CreateAnalysisInfo(analysisId, projectId, [], []);
        _getAnalysis
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(analysisInfo)
            );

        var result = await _getAnalysisResultsService.GetConsoleResult(projectId, analysisId);

        result.AsT0.ShouldBe("");
        await _fileModelService
            .Received(0)
            .GetFileAsync(Arg.Any<FileId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenNoAnalysis_WhenGetErrorResults_ThenReturnAnalysisDoesNotExistsError()
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

        var result = await _getAnalysisResultsService.GetErrorResults(projectId, analysisId);

        result.AsT1.ShouldBe(new AnalysisDoesNotExistsError(analysisId));
    }

    [Fact]
    public async Task GivenAnalysis_WhenGetErrorResults_ThenErrorResults()
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
                    "run error",
                    RunResultDefaultTypes.RunError,
                    DateTimeOffset.UtcNow
                ),
            ],
            [new AnalysisError("analysis error")]
        );
        _getAnalysis
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(analysisInfo)
            );
        _fileModelService
            .GetFileAsync(resultId.ToFileId(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Stream>(new MemoryStream("error message"u8.ToArray())));

        var result = await _getAnalysisResultsService.GetErrorResults(projectId, analysisId);

        result.AsT0.ShouldBe(
            new List<AnalysisErrorResult>
            {
                new("Analysis Error", "analysis error", AnalysisErrorResult.Major),
                new("run error", "error message", AnalysisErrorResult.Minor),
            }
        );
    }

    [Fact]
    public async Task GivenNoAnalysis_WhenGetFileResults_ThenReturnAnalysisDoesNotExistsError()
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

        var result = await _getAnalysisResultsService.GetFileResults(projectId, analysisId);

        result.AsT1.ShouldBe(new AnalysisDoesNotExistsError(analysisId));
    }

    [Fact]
    public async Task GivenAnalysis_WhenGetFileResults_ThenFileResults()
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
                    "File.txt",
                    RunResultDefaultTypes.FileType,
                    DateTimeOffset.UtcNow
                ),
            ],
            [new AnalysisError("analysis error")]
        );
        _getAnalysis
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(analysisInfo)
            );

        var result = await _getAnalysisResultsService.GetFileResults(projectId, analysisId);

        result.AsT0.ShouldBe(
            new List<AnalysisFileResult> { new(resultId.ToFileId(), "File.txt", "file") }
        );
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
