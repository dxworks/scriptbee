using DxWorks.ScriptBee.Analysis.Sdk.Results;
using NSubstitute;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class HelperFunctionsResultServiceTest
{
    private readonly IResultCollector _resultCollector = Substitute.For<IResultCollector>();

    private readonly IScriptResultsStore _scriptResultsStore =
        Substitute.For<IScriptResultsStore>();

    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly HelperFunctionsResultService _helperFunctionsResultService;

    public HelperFunctionsResultServiceTest()
    {
        _helperFunctionsResultService = new HelperFunctionsResultService(
            _resultCollector,
            _scriptResultsStore,
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

        await _helperFunctionsResultService.UploadResultAsync(
            fileName,
            type,
            content,
            TestContext.Current.CancellationToken
        );

        await _scriptResultsStore
            .Received()
            .UploadFileAsync<object>(
                resultId.ToFileId(),
                Arg.Any<Stream>(),
                null,
                Arg.Any<CancellationToken>()
            );
        _resultCollector.Received().Add(resultId, fileName, type);
    }

    [Fact]
    public async Task UploadResultAsync_StreamContent_UploadsFileAndAddsResult()
    {
        const string fileName = "test.txt";
        const string type = "Console";
        var content = new MemoryStream("test content"u8.ToArray());
        var resultId = new ResultId(Guid.NewGuid());
        _guidProvider.NewGuid().Returns(resultId.Value);

        await _helperFunctionsResultService.UploadResultAsync(
            fileName,
            type,
            content,
            TestContext.Current.CancellationToken
        );

        await _scriptResultsStore
            .Received()
            .UploadFileAsync<object>(
                resultId.ToFileId(),
                content,
                null,
                Arg.Any<CancellationToken>()
            );
        _resultCollector.Received().Add(resultId, fileName, type);
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

        _scriptResultsStore
            .Received()
            .UploadFileAsync<object>(
                resultId.ToFileId(),
                Arg.Any<Stream>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );
        _resultCollector.Received().Add(resultId, fileName, type);
    }

    [Fact]
    public void UploadResult_StreamContent_UploadsFileAndAddsResult()
    {
        const string fileName = "test.txt";
        const string type = "console";
        var content = new MemoryStream([.. "test content"u8]);
        var resultId = new ResultId(Guid.NewGuid());
        _guidProvider.NewGuid().Returns(resultId.Value);

        _helperFunctionsResultService.UploadResult(fileName, type, content);

        _scriptResultsStore
            .Received()
            .UploadFileAsync<object>(
                resultId.ToFileId(),
                content,
                cancellationToken: Arg.Any<CancellationToken>()
            );
        _resultCollector.Received().Add(resultId, fileName, type);
    }
}
