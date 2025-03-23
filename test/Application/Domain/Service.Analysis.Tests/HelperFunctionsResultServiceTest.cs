using NSubstitute;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Files;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class HelperFunctionsResultServiceTest
{
    private readonly HelperFunctionsSettings _helperFunctionsSettings;
    private readonly IResultCollector _resultCollector;
    private readonly IFileModelService _fileModelService;
    private readonly IGuidProvider _guidProvider;
    private readonly HelperFunctionsResultService _helperFunctionsResultService;

    public HelperFunctionsResultServiceTest()
    {
        _helperFunctionsSettings = new HelperFunctionsSettings(
            ProjectId.FromValue("project-id"),
            new AnalysisId("37e8f626-48ad-44a2-8558-5c301c565d20")
        );
        _resultCollector = Substitute.For<IResultCollector>();
        _fileModelService = Substitute.For<IFileModelService>();
        _guidProvider = Substitute.For<IGuidProvider>();
        _helperFunctionsResultService = new HelperFunctionsResultService(
            _helperFunctionsSettings,
            _resultCollector,
            _fileModelService,
            _guidProvider
        );
    }

    [Fact]
    public async Task UploadResultAsync_StringContent_UploadsFileAndAddsResult()
    {
        const string fileName = "test.txt";
        const string type = "Console";
        const string content = "test content";
        var resultId = new ResultId(Guid.NewGuid());
        _guidProvider.NewGuid().Returns(resultId.Value);

        await _helperFunctionsResultService.UploadResultAsync(fileName, type, content);

        await _fileModelService
            .Received()
            .UploadFileAsync(resultId.ToFileId(), Arg.Any<Stream>(), Arg.Any<CancellationToken>());
        _resultCollector.Received().Add(resultId, _helperFunctionsSettings, fileName, type);
    }

    [Fact]
    public async Task UploadResultAsync_StreamContent_UploadsFileAndAddsResult()
    {
        const string fileName = "test.txt";
        const string type = "Console";
        var content = new MemoryStream("test content"u8.ToArray());
        var resultId = new ResultId(Guid.NewGuid());
        _guidProvider.NewGuid().Returns(resultId.Value);

        await _helperFunctionsResultService.UploadResultAsync(fileName, type, content);

        await _fileModelService
            .Received()
            .UploadFileAsync(resultId.ToFileId(), content, Arg.Any<CancellationToken>());
        _resultCollector.Received().Add(resultId, _helperFunctionsSettings, fileName, type);
    }

    [Fact]
    public void UploadResult_StringContent_UploadsFileAndAddsResult()
    {
        const string fileName = "test.txt";
        const string type = "text/plain";
        const string content = "test content";
        var resultId = new ResultId(Guid.NewGuid());
        _guidProvider.NewGuid().Returns(resultId.Value);

        _helperFunctionsResultService.UploadResult(fileName, type, content);

        _fileModelService.Received().UploadFile(resultId.ToFileId(), Arg.Any<Stream>());
        _resultCollector.Received().Add(resultId, _helperFunctionsSettings, fileName, type);
    }

    [Fact]
    public void UploadResult_StreamContent_UploadsFileAndAddsResult()
    {
        const string fileName = "test.txt";
        const string type = "console";
        var content = new MemoryStream("test content"u8.ToArray());
        var resultId = new ResultId(Guid.NewGuid());
        _guidProvider.NewGuid().Returns(resultId.Value);

        _helperFunctionsResultService.UploadResult(fileName, type, content);

        _fileModelService.Received().UploadFile(resultId.ToFileId(), content);
        _resultCollector.Received().Add(resultId, _helperFunctionsSettings, fileName, type);
    }
}
