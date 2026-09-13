using DxWorks.ScriptBee.Analysis.Sdk.Model;
using NSubstitute;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.File;

namespace DxWorks.ScriptBee.Analysis.Sdk.Tests.Model;

public class ModelFileLoaderTests
{
    private readonly IFileModelService _fileModelService = Substitute.For<IFileModelService>();

    private readonly ModelFileLoader _loader;

    public ModelFileLoaderTests()
    {
        _loader = new ModelFileLoader(_fileModelService);
    }

    [Fact]
    public async Task LoadModelFileStreamAsync_WhenCalled_UsesFileModelService()
    {
        var fileId = new FileId("d245c7e2-1ec6-4b65-8672-d37ec5d31311");
        using var expectedStream = new MemoryStream([1, 2, 3, 4]);
        _fileModelService
            .GetFileAsync(fileId, Arg.Any<CancellationToken>())
            .Returns(expectedStream);

        var result = await _loader.LoadModelFileStreamAsync(
            fileId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeSameAs(expectedStream);
        await _fileModelService.Received(1).GetFileAsync(fileId, Arg.Any<CancellationToken>());
    }
}
