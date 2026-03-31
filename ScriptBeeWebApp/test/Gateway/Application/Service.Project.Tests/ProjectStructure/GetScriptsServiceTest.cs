using NSubstitute;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Project.ProjectStructure;

namespace ScriptBee.Service.Project.Tests.ProjectStructure;

public class GetScriptsServiceTest
{
    private readonly IGetScripts _getScripts = Substitute.For<IGetScripts>();

    private readonly GetScriptsService _getScriptsService;

    public GetScriptsServiceTest()
    {
        _getScriptsService = new GetScriptsService(_getScripts);
    }

    [Fact]
    public async Task GetAllScriptsForAProject()
    {
        var projectId = ProjectId.FromValue("id");
        var expectedScripts = new List<Script>
        {
            new(
                new ScriptId(Guid.NewGuid()),
                projectId,
                "name",
                "file-path",
                "absolute-file-path",
                new ScriptLanguage("language", "extension"),
                []
            ),
        };
        _getScripts
            .GetAll(projectId, TestContext.Current.CancellationToken)
            .Returns(expectedScripts);

        var scripts = await _getScriptsService.GetAll(
            projectId,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(expectedScripts, scripts);
    }
}
