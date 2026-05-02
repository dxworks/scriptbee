using NSubstitute;
using OneOf;
using ScriptBee.Analysis;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.UseCases.Gateway.Analysis;

namespace ScriptBee.Service.Gateway.Tests.Analysis;

public class DeleteAnalysisServiceTest
{
    private readonly IGetAnalysis _getAnalysis = Substitute.For<IGetAnalysis>();
    private readonly IDeleteAnalysis _deleteAnalysis = Substitute.For<IDeleteAnalysis>();
    private readonly IFileModelService _fileModelService = Substitute.For<IFileModelService>();
    private readonly DeleteAnalysisService _service;

    public DeleteAnalysisServiceTest()
    {
        _service = new DeleteAnalysisService(_getAnalysis, _deleteAnalysis, _fileModelService);
    }

    [Fact]
    public async Task GivenExistingAnalysis_WhenDelete_ShouldDeleteFilesAndAnalysis()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId(Guid.NewGuid());
        var resultId1 = new ResultId(Guid.NewGuid());
        var resultId2 = new ResultId(Guid.NewGuid());
        var scriptFileId = new FileId(Guid.NewGuid());

        var analysisInfo = new AnalysisInfo(
            analysisId,
            projectId,
            new InstanceId(Guid.NewGuid()),
            new ScriptId(Guid.NewGuid()),
            scriptFileId,
            AnalysisStatus.Finished,
            new List<ResultSummary>
            {
                new(resultId1, "result1", "type1", DateTimeOffset.Now),
                new(resultId2, "result2", "type2", DateTimeOffset.Now),
            },
            new List<AnalysisError>(),
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow
        );

        _getAnalysis
            .GetById(analysisId, TestContext.Current.CancellationToken)
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(analysisInfo)
            );

        var command = new DeleteAnalysisCommand(projectId, analysisId);

        // Act
        await _service.Delete(command, TestContext.Current.CancellationToken);

        // Assert
        await _fileModelService
            .Received(1)
            .DeleteFilesAsync(
                Arg.Is<IEnumerable<FileId>>(ids =>
                    ContainsFileIdsToDelete(ids, resultId1, resultId2, scriptFileId)
                ),
                TestContext.Current.CancellationToken
            );
        await _deleteAnalysis
            .Received(1)
            .DeleteById(analysisId, TestContext.Current.CancellationToken);
    }

    private static bool ContainsFileIdsToDelete(
        IEnumerable<FileId> ids,
        ResultId resultId1,
        ResultId resultId2,
        FileId scriptFileId
    )
    {
        var fileIds = ids.ToList();
        return fileIds.Count == 3
            && fileIds.Contains(resultId1.ToFileId())
            && fileIds.Contains(resultId2.ToFileId())
            && fileIds.Contains(scriptFileId);
    }

    [Fact]
    public async Task GivenNonExistingAnalysis_WhenDelete_ShouldDoNothing()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId(Guid.NewGuid());

        _getAnalysis
            .GetById(analysisId, TestContext.Current.CancellationToken)
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(
                    new AnalysisDoesNotExistsError(analysisId)
                )
            );

        var command = new DeleteAnalysisCommand(projectId, analysisId);

        // Act
        await _service.Delete(command, TestContext.Current.CancellationToken);

        // Assert
        await _fileModelService
            .DidNotReceiveWithAnyArgs()
            .DeleteFilesAsync(null!, TestContext.Current.CancellationToken);
        await _deleteAnalysis
            .DidNotReceiveWithAnyArgs()
            .DeleteById(default!, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GivenAnalysisWithoutFiles_WhenDelete_ShouldOnlyDeleteAnalysis()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId(Guid.NewGuid());

        var analysisInfo = new AnalysisInfo(
            analysisId,
            projectId,
            new InstanceId(Guid.NewGuid()),
            new ScriptId(Guid.NewGuid()),
            null,
            AnalysisStatus.Finished,
            new List<ResultSummary>(),
            new List<AnalysisError>(),
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow
        );

        _getAnalysis
            .GetById(analysisId, TestContext.Current.CancellationToken)
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(analysisInfo)
            );

        var command = new DeleteAnalysisCommand(projectId, analysisId);

        // Act
        await _service.Delete(command, TestContext.Current.CancellationToken);

        // Assert
        await _fileModelService
            .DidNotReceiveWithAnyArgs()
            .DeleteFilesAsync(null!, TestContext.Current.CancellationToken);
        await _deleteAnalysis
            .Received(1)
            .DeleteById(analysisId, TestContext.Current.CancellationToken);
    }
}
