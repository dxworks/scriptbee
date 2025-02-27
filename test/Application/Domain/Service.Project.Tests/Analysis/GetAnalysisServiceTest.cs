using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project.Analysis;
using ScriptBee.Service.Project.Analysis;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class GetAnalysisServiceTest
{
    private readonly IGetAllAnalysisResults _getAllAnalysisResults =
        Substitute.For<IGetAllAnalysisResults>();

    private readonly IGetAnalysisResult _getAnalysisResult = Substitute.For<IGetAnalysisResult>();

    private readonly GetAnalysisService _getAnalysisService;

    public GetAnalysisServiceTest()
    {
        _getAnalysisService = new GetAnalysisService(_getAllAnalysisResults, _getAnalysisResult);
    }

    [Fact]
    public async Task GetAllAnalyses()
    {
        var projectId = ProjectId.FromValue("project-id");
        IEnumerable<AnalysisResult> expectedAnalysisResults =
        [
            AnalysisResult.Started(
                AnalysisId.FromGuid(Guid.NewGuid()),
                new InstanceInfo(
                    InstanceId.FromGuid(Guid.NewGuid()),
                    projectId,
                    "http://url:8080",
                    DateTimeOffset.UtcNow
                ),
                new AnalysisMetadata([], []),
                DateTimeOffset.Now
            ),
        ];
        _getAllAnalysisResults.GetAll(projectId).Returns(Task.FromResult(expectedAnalysisResults));

        var analysisResults = await _getAnalysisService.GetAll(projectId);

        analysisResults.ShouldBeEquivalentTo(expectedAnalysisResults);
    }

    [Fact]
    public async Task GetAnalysis()
    {
        var analysisId = AnalysisId.FromGuid(Guid.NewGuid());
        var expectedAnalysis = AnalysisResult.Started(
            AnalysisId.FromGuid(Guid.NewGuid()),
            new InstanceInfo(
                InstanceId.FromGuid(Guid.NewGuid()),
                ProjectId.FromValue("project-id"),
                "http://url:8080",
                DateTimeOffset.UtcNow
            ),
            new AnalysisMetadata([], []),
            DateTimeOffset.Now
        );
        _getAnalysisResult
            .GetById(analysisId)
            .Returns(
                Task.FromResult<OneOf<AnalysisResult, AnalysisDoesNotExistsError>>(expectedAnalysis)
            );

        var analysisResult = await _getAnalysisService.GetById(analysisId);

        analysisResult.ShouldBe(expectedAnalysis);
    }

    [Fact]
    public async Task GivenNoAnalysis_ShouldReturnAnalysisDoesNotExistsError()
    {
        var analysisId = AnalysisId.FromGuid(Guid.NewGuid());
        var expectedError = new AnalysisDoesNotExistsError(analysisId);
        _getAnalysisResult
            .GetById(analysisId)
            .Returns(
                Task.FromResult<OneOf<AnalysisResult, AnalysisDoesNotExistsError>>(expectedError)
            );

        var analysisResult = await _getAnalysisService.GetById(analysisId);

        analysisResult.ShouldBe(expectedError);
    }
}
