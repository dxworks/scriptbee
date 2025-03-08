using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Analysis;
using ScriptBee.Service.Project.Analysis;

namespace ScriptBee.Service.Project.Tests.Analysis;

public class GetAnalysisServiceTest
{
    private readonly IGetAllAnalyses _getAllAnalyses = Substitute.For<IGetAllAnalyses>();

    private readonly IGetAnalysis _getAnalysis = Substitute.For<IGetAnalysis>();

    private readonly GetAnalysisService _getAnalysisService;

    public GetAnalysisServiceTest()
    {
        _getAnalysisService = new GetAnalysisService(_getAllAnalyses, _getAnalysis);
    }

    [Fact]
    public async Task GetAllAnalyses()
    {
        var projectId = ProjectId.FromValue("project-id");
        IEnumerable<AnalysisInfo> expectedAnalysisResults =
        [
            AnalysisInfo.Started(new AnalysisId(Guid.NewGuid()), projectId, DateTimeOffset.Now),
        ];
        _getAllAnalyses.GetAll(projectId).Returns(Task.FromResult(expectedAnalysisResults));

        var analysisResults = await _getAnalysisService.GetAll(projectId);

        analysisResults.ShouldBeEquivalentTo(expectedAnalysisResults);
    }

    [Fact]
    public async Task GetAnalysis()
    {
        var analysisId = new AnalysisId(Guid.NewGuid());
        var expectedAnalysis = AnalysisInfo.Started(
            new AnalysisId(Guid.NewGuid()),
            ProjectId.FromValue("project-id"),
            DateTimeOffset.Now
        );
        _getAnalysis
            .GetById(analysisId)
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(expectedAnalysis)
            );

        var analysisResult = await _getAnalysisService.GetById(analysisId);

        analysisResult.ShouldBe(expectedAnalysis);
    }

    [Fact]
    public async Task GivenNoAnalysis_ShouldReturnAnalysisDoesNotExistsError()
    {
        var analysisId = new AnalysisId(Guid.NewGuid());
        var expectedError = new AnalysisDoesNotExistsError(analysisId);
        _getAnalysis
            .GetById(analysisId)
            .Returns(
                Task.FromResult<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>>(expectedError)
            );

        var analysisResult = await _getAnalysisService.GetById(analysisId);

        analysisResult.ShouldBe(expectedError);
    }
}
