using System.Text;
using NSubstitute;
using ScriptBee.Analysis;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Gateway.Analysis;
using ScriptBee.UseCases.Gateway.ProjectStructure;

namespace ScriptBee.Service.Gateway.Tests.Analysis;

public class GetAnalysisScriptServiceTest
{
    private readonly IGetAnalysis _getAnalysisMock = Substitute.For<IGetAnalysis>();
    private readonly IFileModelService _fileModelServiceMock = Substitute.For<IFileModelService>();

    private readonly IGetScriptsUseCase _getScriptsUseCaseMock =
        Substitute.For<IGetScriptsUseCase>();

    private readonly GetAnalysisScriptService _service;

    public GetAnalysisScriptServiceTest()
    {
        _service = new GetAnalysisScriptService(
            _getAnalysisMock,
            _fileModelServiceMock,
            _getScriptsUseCaseMock
        );
    }

    [Fact]
    public async Task GivenValidAnalysisId_WhenGetScriptContent_ThenReturnsContent()
    {
        // Arrange
        var scriptId = new ScriptId(Guid.NewGuid());
        var projectId = ProjectId.FromValue("projectId");
        var scriptFileId = new FileId(Guid.NewGuid());
        var analysisId = new AnalysisId(Guid.NewGuid());
        var analysisInfo = new AnalysisInfo(
            analysisId,
            projectId,
            new InstanceId(Guid.NewGuid()),
            scriptId,
            scriptFileId,
            AnalysisStatus.Finished,
            [],
            [],
            DateTimeOffset.Now,
            DateTimeOffset.Now
        );

        const string content = "print('hello')";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        _getAnalysisMock.GetById(analysisId, Arg.Any<CancellationToken>()).Returns(analysisInfo);
        _fileModelServiceMock
            .GetFileAsync(scriptFileId, Arg.Any<CancellationToken>())
            .Returns(stream);

        // Act
        var result = await _service.GetScriptContent(
            analysisId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.AsT0.ShouldBe(content);
    }

    [Fact]
    public async Task GivenNonExistentAnalysisId_WhenGetScriptContent_ThenReturnsAnalysisDoesNotExistsError()
    {
        var analysisId = new AnalysisId(Guid.NewGuid());
        var scriptId = new ScriptId(Guid.NewGuid());
        _getAnalysisMock
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(new AnalysisDoesNotExistsError(analysisId));

        var result = await _service.GetScriptContent(
            analysisId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new AnalysisDoesNotExistsError(analysisId));
    }

    [Fact]
    public async Task GivenAnalysisWithNoScriptFile_WhenGetScriptContent_ThenReturnsScriptFileDoesNotExistError()
    {
        var analysisId = new AnalysisId(Guid.NewGuid());
        var scriptId = new ScriptId(Guid.NewGuid());
        var analysisInfo = new AnalysisInfo(
            analysisId,
            ProjectId.FromValue("projectId"),
            new InstanceId(Guid.NewGuid()),
            scriptId,
            null,
            AnalysisStatus.Finished,
            [],
            [],
            DateTimeOffset.Now,
            DateTimeOffset.Now
        );

        _getAnalysisMock.GetById(analysisId, Arg.Any<CancellationToken>()).Returns(analysisInfo);

        var result = await _service.GetScriptContent(
            analysisId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        result.AsT2.ShouldBe(new ScriptDoesNotExistsError(scriptId));
    }

    [Fact]
    public async Task GivenValidAnalysisId_WhenGetScriptMetadata_ThenReturnsMetadata()
    {
        // Arrange
        var scriptId = new ScriptId(Guid.NewGuid());
        var projectId = ProjectId.FromValue("projectId");
        var scriptFileId = new FileId(Guid.NewGuid());
        var analysisId = new AnalysisId(Guid.NewGuid());
        var analysisInfo = new AnalysisInfo(
            analysisId,
            projectId,
            new InstanceId(Guid.NewGuid()),
            scriptId,
            scriptFileId,
            AnalysisStatus.Finished,
            [],
            [],
            DateTimeOffset.Now,
            DateTimeOffset.Now
        );
        var script = new Script(
            scriptId,
            projectId,
            new ProjectStructureFile("path"),
            new ScriptLanguage("Python", ".py"),
            []
        );
        _getAnalysisMock.GetById(analysisId, Arg.Any<CancellationToken>()).Returns(analysisInfo);
        _getScriptsUseCaseMock
            .GetById(projectId, scriptId, Arg.Any<CancellationToken>())
            .Returns(script);

        // Act
        var result = await _service.GetFileScript(
            analysisId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.AsT0.ShouldBe(script);
    }

    [Fact]
    public async Task GivenNoAnalysis_WhenGetScriptMetadata_ThenReturnsAnalysisDoesNotExistsError()
    {
        // Arrange
        var analysisId = new AnalysisId(Guid.NewGuid());
        var scriptId = new ScriptId(Guid.NewGuid());

        _getAnalysisMock
            .GetById(analysisId, Arg.Any<CancellationToken>())
            .Returns(new AnalysisDoesNotExistsError(analysisId));

        var result = await _service.GetFileScript(
            analysisId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new AnalysisDoesNotExistsError(analysisId));
    }

    [Fact]
    public async Task GivenNoScript_WhenGetScriptMetadata_ThenReturnsScriptDoesNotExistsError()
    {
        // Arrange
        var scriptId = new ScriptId(Guid.NewGuid());
        var projectId = ProjectId.FromValue("projectId");
        var scriptFileId = new FileId(Guid.NewGuid());
        var analysisId = new AnalysisId(Guid.NewGuid());
        var analysisInfo = new AnalysisInfo(
            analysisId,
            projectId,
            new InstanceId(Guid.NewGuid()),
            scriptId,
            scriptFileId,
            AnalysisStatus.Finished,
            [],
            [],
            DateTimeOffset.Now,
            DateTimeOffset.Now
        );

        _getAnalysisMock.GetById(analysisId, Arg.Any<CancellationToken>()).Returns(analysisInfo);
        _getScriptsUseCaseMock
            .GetById(projectId, scriptId, Arg.Any<CancellationToken>())
            .Returns(new ScriptDoesNotExistsError(scriptId));

        var result = await _service.GetFileScript(
            analysisId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        result.AsT2.ShouldBe(new ScriptDoesNotExistsError(scriptId));
    }
}
