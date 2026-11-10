using System.Text;
using DxWorks.ScriptBee.Plugin.Api.Services;
using NSubstitute;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Default.Tests;

public class CsvHelperFunctionsTests
{
    private readonly IHelperFunctionsResultService _resultService =
        Substitute.For<IHelperFunctionsResultService>();
    private readonly CsvHelperFunctions _csvHelperFunctions;

    public CsvHelperFunctionsTests()
    {
        _csvHelperFunctions = new CsvHelperFunctions(_resultService);
    }

    [Fact]
    public async Task ExportCsvAsync_WritesRecordsToCsvAndUploadsResult()
    {
        var records = new List<TestRecord> { new("Alice", 30), new("Bob", 25) };
        string? capturedContent = null;

        await _resultService.UploadResultAsync(
            "test.csv",
            RunResultDefaultTypes.FileType,
            Arg.Do<Stream>(stream =>
            {
                using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
                capturedContent = reader.ReadToEnd();
            }),
            TestContext.Current.CancellationToken
        );

        await _csvHelperFunctions.ExportCsvAsync(
            "test.csv",
            records,
            TestContext.Current.CancellationToken
        );

        // Assert
        await _resultService
            .Received(1)
            .UploadResultAsync(
                "test.csv",
                RunResultDefaultTypes.FileType,
                Arg.Any<Stream>(),
                TestContext.Current.CancellationToken
            );
        capturedContent.ShouldNotBeNull();
        capturedContent.ShouldContain("Name,Age");
        capturedContent.ShouldContain("Alice,30");
        capturedContent.ShouldContain("Bob,25");
    }

    [Fact]
    public void ExportCsv_WritesRecordsToCsvAndUploadsResult()
    {
        var records = new List<object>
        {
            new TestRecord("Charlie", 40),
            new TestRecord("Diana", 35),
        };
        string? capturedContent = null;

        _resultService.UploadResult(
            "sync.csv",
            RunResultDefaultTypes.FileType,
            Arg.Do<Stream>(stream =>
            {
                using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
                capturedContent = reader.ReadToEnd();
            })
        );

        _csvHelperFunctions.ExportCsv("sync.csv", records);

        // Assert
        _resultService
            .Received(1)
            .UploadResult("sync.csv", RunResultDefaultTypes.FileType, Arg.Any<Stream>());
        capturedContent.ShouldNotBeNull();
        capturedContent.ShouldContain("Name,Age");
        capturedContent.ShouldContain("Charlie,40");
        capturedContent.ShouldContain("Diana,35");
    }

    [Fact]
    public void ExportCsv_WithDictionaryRecords_WritesRecordsToCsvAndUploadsResult()
    {
        var records = new List<object>
        {
            new Dictionary<string, object?> { ["Name"] = "Eve", ["Score"] = 95 },
            new Dictionary<string, object?> { ["Name"] = "Frank", ["Score"] = 88 },
        };
        string? capturedContent = null;

        _resultService.UploadResult(
            "dict.csv",
            RunResultDefaultTypes.FileType,
            Arg.Do<Stream>(stream =>
            {
                using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
                capturedContent = reader.ReadToEnd();
            })
        );

        _csvHelperFunctions.ExportCsv("dict.csv", records);

        // Assert
        _resultService
            .Received(1)
            .UploadResult("dict.csv", RunResultDefaultTypes.FileType, Arg.Any<Stream>());
        capturedContent.ShouldNotBeNull();
        capturedContent.ShouldContain("Name,Score");
        capturedContent.ShouldContain("Eve,95");
        capturedContent.ShouldContain("Frank,88");
    }

    public sealed record TestRecord(string Name, int Age);
}
