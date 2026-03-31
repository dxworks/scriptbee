using NSubstitute;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Project.ProjectStructure;

namespace ScriptBee.Service.Project.Tests.ProjectStructure;

public class GetScriptsServiceTest
{
    private readonly IGetScripts _getScripts = Substitute.For<IGetScripts>();
    private readonly ILoadFile _loadFile = Substitute.For<ILoadFile>();

    private readonly GetScriptsService _getScriptsService;

    public GetScriptsServiceTest()
    {
        _getScriptsService = new GetScriptsService(_getScripts, _loadFile);
    }

    [Fact]
    public async Task GetAllScriptsForAProject()
    {
        var projectId = ProjectId.FromValue("id");
        var expectedScripts = new List<Script>
        {
            CreateScript(projectId, new ScriptId(Guid.NewGuid())),
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

    [Fact]
    public async Task GivenScript_WhenGetById_ShouldReturnScript()
    {
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var script = CreateScript(projectId, scriptId);

        _getScripts.Get(scriptId, TestContext.Current.CancellationToken).Returns(script);

        var result = await _getScriptsService.GetById(
            projectId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(script, result);
    }

    [Fact]
    public async Task GivenScriptDoesNotExistsError_WhenGetById_ShouldReturnError()
    {
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var error = new ScriptDoesNotExistsError(scriptId);
        _getScripts.Get(scriptId, TestContext.Current.CancellationToken).Returns(error);

        var result = await _getScriptsService.GetById(
            projectId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(error, result);
    }

    [Fact]
    public async Task GivenScriptDoesNotExistsError_WhenGetContent_ShouldReturnError()
    {
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var error = new ScriptDoesNotExistsError(scriptId);
        _getScripts.Get(scriptId, TestContext.Current.CancellationToken).Returns(error);

        var result = await _getScriptsService.GetScriptContent(
            projectId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(error, result);
    }

    [Fact]
    public async Task GivenFileDoesNotExistsError_WhenGetContent_ShouldReturnError()
    {
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var script = CreateScript(projectId, scriptId);
        _getScripts.Get(scriptId, TestContext.Current.CancellationToken).Returns(script);
        _loadFile
            .GetScriptContent(projectId, script.FilePath, TestContext.Current.CancellationToken)
            .Returns(new FileDoesNotExistsError(script.FilePath));

        var result = await _getScriptsService.GetScriptContent(
            projectId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(new ScriptDoesNotExistsError(scriptId), result);
    }

    [Fact]
    public async Task GivenScript_WhenGetContent_ShouldReturnContent()
    {
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var script = CreateScript(projectId, scriptId);
        _getScripts.Get(scriptId, TestContext.Current.CancellationToken).Returns(script);
        _loadFile
            .GetScriptContent(projectId, script.FilePath, TestContext.Current.CancellationToken)
            .Returns("content");

        var result = await _getScriptsService.GetScriptContent(
            projectId,
            scriptId,
            TestContext.Current.CancellationToken
        );

        Assert.Equal("content", result);
    }

    private static Script CreateScript(ProjectId projectId, ScriptId scriptId)
    {
        return new Script(
            scriptId,
            projectId,
            "name",
            "file-path",
            "absolute-file-path",
            new ScriptLanguage("language", "extension"),
            []
        );
    }
}
