using DxWorks.ScriptBee.Analysis.Sdk.Results;
using NSubstitute;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.File;

namespace DxWorks.ScriptBee.Analysis.Sdk.Tests.Results;

public class AnalysisResultServiceTests
{
    private static readonly Guid FixedGuid = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly DateTimeOffset FixedDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly IScriptResultsStore _store = Substitute.For<IScriptResultsStore>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly List<ResultSummary> _recordedSummaries = [];

    private readonly AnalysisResultService _service;

    public AnalysisResultServiceTests()
    {
        _guidProvider.NewGuid().Returns(FixedGuid);
        _dateTimeProvider.UtcNow().Returns(FixedDate);

        _service = new AnalysisResultService(
            _store,
            _guidProvider,
            _dateTimeProvider,
            _recordedSummaries.Add
        );
    }

    [Fact]
    public async Task AddFileAsync_WithStreamContent_UploadsToStoreAndNotifies()
    {
        // Arrange
        await using var content = new MemoryStream("file content"u8.ToArray());

        // Act
        var resultId = await _service.AddFileAsync(
            "report.txt",
            content,
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(new ResultId(FixedGuid), resultId);
        await _store
            .Received(1)
            .UploadFileAsync<object>(
                new FileId(FixedGuid),
                content,
                null,
                Arg.Any<CancellationToken>()
            );
        var summary = Assert.Single(_recordedSummaries);
        Assert.Equal("report.txt", summary.Name);
        Assert.Equal(RunResultTypes.File, summary.Type);
        Assert.Equal(FixedDate, summary.CreationDate);
    }

    [Fact]
    public async Task AddFileAsync_WithStringContent_NotifiesWithCorrectMetadata()
    {
        // Act
        var resultId = await _service.AddFileAsync(
            "output.txt",
            "hello world",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(new ResultId(FixedGuid), resultId);
        await _store
            .Received(1)
            .UploadFileAsync<object>(
                new FileId(FixedGuid),
                Arg.Any<Stream>(),
                null,
                Arg.Any<CancellationToken>()
            );
        var summary = Assert.Single(_recordedSummaries);
        Assert.Equal("output.txt", summary.Name);
        Assert.Equal(RunResultTypes.File, summary.Type);
    }

    [Fact]
    public async Task AddConsoleAsync_UploadsWithConsoleTypeAndDefaultName()
    {
        // Act
        var resultId = await _service.AddConsoleAsync(
            "log output",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(new ResultId(FixedGuid), resultId);
        var summary = Assert.Single(_recordedSummaries);
        Assert.Equal("ConsoleOutput", summary.Name);
        Assert.Equal(RunResultTypes.Console, summary.Type);
    }

    [Fact]
    public async Task AddErrorAsync_UploadsWithRunErrorTypeAndDefaultName()
    {
        // Act
        var resultId = await _service.AddErrorAsync(
            "something failed",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(new ResultId(FixedGuid), resultId);
        var summary = Assert.Single(_recordedSummaries);
        Assert.Equal("RunError", summary.Name);
        Assert.Equal(RunResultTypes.RunError, summary.Type);
    }

    [Fact]
    public async Task AddResultAsync_WithCustomType_NotifiesWithCorrectSummary()
    {
        // Arrange
        await using var content = new MemoryStream("chart data"u8.ToArray());

        // Act
        await _service.AddResultAsync(
            "MyChart",
            "Chart",
            content,
            TestContext.Current.CancellationToken
        );

        // Assert
        var summary = Assert.Single(_recordedSummaries);
        Assert.Equal("MyChart", summary.Name);
        Assert.Equal("Chart", summary.Type);
        Assert.Equal(FixedDate, summary.CreationDate);
    }

    [Fact]
    public async Task AddResultAsync_WithoutCallback_DoesNotThrow()
    {
        // Arrange
        var serviceWithoutCallback = new AnalysisResultService(
            _store,
            _guidProvider,
            _dateTimeProvider
        );
        await using var content = new MemoryStream("data"u8.ToArray());

        // Act
        var exception = await Record.ExceptionAsync(() =>
            serviceWithoutCallback.AddResultAsync(
                "name",
                RunResultTypes.File,
                content,
                TestContext.Current.CancellationToken
            )
        );

        // Assert
        Assert.Null(exception);
    }
}
