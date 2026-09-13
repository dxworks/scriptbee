using DxWorks.ScriptBee.Analysis.Sdk.Scripts;
using NSubstitute;
using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace DxWorks.ScriptBee.Analysis.Sdk.Tests.Scripts;

public class ScriptLoaderTests
{
    private readonly IGetScripts _getScripts = Substitute.For<IGetScripts>();
    private readonly ILoadFile _loadFile = Substitute.For<ILoadFile>();

    private readonly ScriptLoader _loader;

    public ScriptLoaderTests()
    {
        _loader = new ScriptLoader(_loadFile, _getScripts);
    }

    [Fact]
    public async Task Get_WhenScriptExists_ReturnsScript()
    {
        var scriptId = new ScriptId("7e715fe3-6f10-48d0-92a1-3d960d32d7c4");
        var projectId = ProjectId.FromValue("project-id");
        var script = CreateScript(scriptId, projectId);

        _getScripts.Get(scriptId, Arg.Any<CancellationToken>()).Returns(script);

        var result = await _loader.Get(scriptId, TestContext.Current.CancellationToken);

        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBeSameAs(script);
        await _getScripts.Received(1).Get(scriptId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetScriptContent_WhenFileExists_ReturnsContent()
    {
        var projectId = ProjectId.FromValue("project-id");
        const string path = "script/test.cs";
        const string content = "Console.WriteLine(\"hello\");";
        _loadFile
            .GetScriptContent(projectId, path, Arg.Any<CancellationToken>())
            .Returns(OneOf<string, FileDoesNotExistsError>.FromT0(content));

        var result = await _loader.GetScriptContent(
            projectId,
            path,
            TestContext.Current.CancellationToken
        );

        result.IsT0.ShouldBeTrue();
        result.AsT0.ShouldBe(content);
    }

    [Fact]
    public async Task GetScriptContent_WhenFileDoesNotExist_ReturnsScriptDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        const string path = "script/missing.cs";
        _loadFile
            .GetScriptContent(projectId, path, Arg.Any<CancellationToken>())
            .Returns(
                OneOf<string, FileDoesNotExistsError>.FromT1(new FileDoesNotExistsError(path))
            );

        var result = await _loader.GetScriptContent(
            projectId,
            path,
            TestContext.Current.CancellationToken
        );

        result.IsT1.ShouldBeTrue();
        result.AsT1.ToString().ShouldBe($"Script at path '{path}' does not exist.");
    }

    private static Script CreateScript(ScriptId scriptId, ProjectId projectId)
    {
        return new Script(
            scriptId,
            projectId,
            new ProjectStructureFile("src/test.cs"),
            new ScriptLanguage("csharp", ".cs"),
            []
        );
    }
}
