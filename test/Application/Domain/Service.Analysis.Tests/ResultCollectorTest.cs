using NSubstitute;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class ResultCollectorTest
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly ResultCollector _resultCollector;

    public ResultCollectorTest()
    {
        _resultCollector = new ResultCollector(_dateTimeProvider);
    }

    [Fact]
    public void Add_AddsResultSummaryToList()
    {
        var resultId = new ResultId(Guid.NewGuid());
        const string outputFileName = "test.txt";
        const string type = "type";
        var utcNow = DateTime.UtcNow;

        _dateTimeProvider.UtcNow().Returns(utcNow);

        _resultCollector.Add(resultId, outputFileName, type);
        var results = _resultCollector.GetResults();

        Assert.Single(results);
        Assert.Equal(resultId, results[0].Id);
        Assert.Equal(type, results[0].Type);
        Assert.Equal(outputFileName, results[0].Name);
        Assert.Equal(utcNow, results[0].CreationDate);
    }

    [Fact]
    public void GetResults_ReturnsListOfResultSummaries()
    {
        var resultId1 = new ResultId(Guid.NewGuid());
        const string type1 = "type-1";
        var utcNow1 = DateTime.UtcNow;
        var resultId2 = new ResultId(Guid.NewGuid());
        const string type2 = "type-2";
        var utcNow2 = DateTime.UtcNow.AddMinutes(1);
        _dateTimeProvider.UtcNow().Returns(utcNow1, utcNow2);

        _resultCollector.Add(resultId1, "file1.txt", type1);
        _resultCollector.Add(resultId2, "file2.json", type2);

        var results = _resultCollector.GetResults();

        Assert.Equal(2, results.Count);
        Assert.Equal(resultId1, results[0].Id);
        Assert.Equal(resultId2, results[1].Id);
        Assert.Equal("file1.txt", results[0].Name);
        Assert.Equal("file2.json", results[1].Name);
        Assert.Equal(utcNow1, results[0].CreationDate);
        Assert.Equal(utcNow2, results[1].CreationDate);
    }

    [Fact]
    public void GetResults_EmptyList_ReturnsEmptyList()
    {
        var results = _resultCollector.GetResults();

        Assert.Empty(results);
    }
}
