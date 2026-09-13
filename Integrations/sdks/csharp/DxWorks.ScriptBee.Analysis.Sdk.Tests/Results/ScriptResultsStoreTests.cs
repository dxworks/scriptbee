using DxWorks.ScriptBee.Analysis.Sdk.Results;
using NSubstitute;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.File;

namespace DxWorks.ScriptBee.Analysis.Sdk.Tests.Results;

public class ScriptResultsStoreTests
{
    private readonly IFileModelService _fileModelService = Substitute.For<IFileModelService>();

    private readonly ScriptResultsStore _scriptResultsStore;

    public ScriptResultsStoreTests()
    {
        _scriptResultsStore = new ScriptResultsStore(_fileModelService);
    }

    [Fact]
    public async Task UploadFileAsync_WhenCalled_DelegatesToFileModelService()
    {
        var fileId = new FileId("0fd7a3a6-733a-4f1e-9bd0-7b4b981400c3");
        using var fileStream = new MemoryStream([9, 8, 7]);
        var metadata = new ScriptResultMetadata { };

        await _scriptResultsStore.UploadFileAsync(
            fileId,
            fileStream,
            metadata,
            TestContext.Current.CancellationToken
        );

        await _fileModelService
            .Received(1)
            .UploadFileAsync(fileId, fileStream, metadata, Arg.Any<CancellationToken>());
    }

    private sealed class ScriptResultMetadata;
}
