using DxWorks.ScriptBee.Plugin.Api.Model;
using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;
using static ScriptBee.Tests.Common.ScriptFixture;

namespace ScriptBee.Service.Project.Tests.ProjectStructure;

public class UpdateScriptServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly IGetScripts _getScripts = Substitute.For<IGetScripts>();
    private readonly IUpdateScript _updateScript = Substitute.For<IUpdateScript>();
    private readonly IUpdateFile _updateFile = Substitute.For<IUpdateFile>();

    private readonly UpdateScriptService _updateScriptService;

    public UpdateScriptServiceTest()
    {
        _updateScriptService = new UpdateScriptService(
            _getProject,
            _getScripts,
            _updateScript,
            _updateFile
        );
    }

    #region Update Script

    [Fact]
    public async Task ProjectDoesNotExists_WhenUpdate()
    {
        var projectId = ProjectId.FromValue("id");
        var error = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(error));

        var result = await _updateScriptService.Update(
            new UpdateScriptCommand(projectId, new ScriptId(Guid.NewGuid()), null, null),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(error);
    }

    [Fact]
    public async Task ScriptDoesNotExists_WhenUpdate()
    {
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var error = new ScriptDoesNotExistsError(scriptId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getScripts
            .Get(scriptId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(error));

        var result = await _updateScriptService.Update(
            new UpdateScriptCommand(
                projectId,
                scriptId,
                null,
                [
                    new ScriptParameter
                    {
                        Name = "parameter",
                        Type = "string",
                        Value = "value",
                    },
                ]
            ),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateScriptNotPerformed_WhenParametersAreNull()
    {
        // Arrange
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var script = BasicScript(projectId, scriptId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getScripts
            .Get(scriptId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(script));

        // Act
        var result = await _updateScriptService.Update(
            new UpdateScriptCommand(projectId, scriptId, null, null),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.AsT0.ShouldBe(script);
        await _updateScript
            .Received(0)
            .Update(Arg.Any<Script>(), TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task UpdateScriptSuccessfullyWithAllParameters()
    {
        // Arrange
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var script = BasicScript(projectId, scriptId);
        var updateScript = BasicScript(projectId, scriptId) with
        {
            File = new ProjectStructureFile("name"),
            Parameters =
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = "string",
                    Value = "value",
                },
            ],
        };
        SetupMocks(projectId, scriptId, script, updateScript);

        // Act
        var result = await UpdateScript(
            projectId,
            scriptId,
            "name",
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = "string",
                    Value = "value",
                },
            ]
        );

        // Assert
        await VerifyUpdateScript(result, updateScript);
        _updateFile.Received(1).RenameFile(projectId, "path.lang", "name");
    }

    [Fact]
    public async Task UpdateScriptSuccessfully_WhenOnlyNameIsUpdated()
    {
        // Arrange
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var script = BasicScript(projectId, scriptId);
        var updateScript = BasicScript(projectId, scriptId) with
        {
            File = new ProjectStructureFile("name"),
        };
        SetupMocks(projectId, scriptId, script, updateScript);

        // Act
        var result = await UpdateScript(projectId, scriptId, "name", null);

        // Assert
        await VerifyUpdateScript(result, updateScript);
        _updateFile.Received(1).RenameFile(projectId, "path.lang", "name");
    }

    [Fact]
    public async Task UpdateScriptSuccessfully_WhenOnlyParametersAreUpdated()
    {
        // Arrange
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var script = BasicScript(projectId, scriptId);
        var updateScript = BasicScript(projectId, scriptId) with
        {
            Parameters =
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = "string",
                    Value = "value",
                },
            ],
        };
        SetupMocks(projectId, scriptId, script, updateScript);

        // Act
        var result = await UpdateScript(
            projectId,
            scriptId,
            null,
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = "string",
                    Value = "value",
                },
            ]
        );

        // Assert
        await VerifyUpdateScript(result, updateScript);
        _updateFile
            .Received(0)
            .RenameFile(Arg.Any<ProjectId>(), Arg.Any<string>(), Arg.Any<string>());
    }

    private async Task VerifyUpdateScript(
        OneOf<Script, ProjectDoesNotExistsError, ScriptDoesNotExistsError> result,
        Script updateScript
    )
    {
        MatchScript(result.AsT0, updateScript);
        await _updateScript
            .Received(1)
            .Update(
                Arg.Is<Script>(x => MatchScript(x, updateScript)),
                TestContext.Current.CancellationToken
            );
    }

    private async Task<
        OneOf<Script, ProjectDoesNotExistsError, ScriptDoesNotExistsError>
    > UpdateScript(
        ProjectId projectId,
        ScriptId scriptId,
        string? name,
        IEnumerable<ScriptParameter>? scriptParameters
    )
    {
        var result = await _updateScriptService.Update(
            new UpdateScriptCommand(projectId, scriptId, name, scriptParameters),
            TestContext.Current.CancellationToken
        );
        return result;
    }

    private void SetupMocks(
        ProjectId projectId,
        ScriptId scriptId,
        Script script,
        Script updateScript
    )
    {
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getScripts
            .Get(scriptId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(script));
        _updateScript.Update(Arg.Any<Script>(), Arg.Any<CancellationToken>()).Returns(updateScript);
    }

    private static bool MatchScript(Script actual, Script expected)
    {
        return actual.Id.Equals(expected.Id)
            && actual.ProjectId.Equals(expected.ProjectId)
            && actual.File.Path.Equals(expected.File.Path)
            && actual.ScriptLanguage.Equals(expected.ScriptLanguage)
            && MatchParameter(actual.Parameters);
    }

    private static bool MatchParameter(IEnumerable<ScriptParameter> parameters)
    {
        return parameters.Single() is { Name: "parameter", Type: "string", Value: "value" };
    }

    #endregion

    #region Update Script Content

    [Fact]
    public async Task ProjectDoesNotExists_WhenUpdateContent()
    {
        var projectId = ProjectId.FromValue("id");
        var error = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(error));

        var result = await _updateScriptService.UpdateContent(
            new UpdateScriptContentCommand(projectId, new ScriptId(Guid.NewGuid()), "content"),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(error);
    }

    [Fact]
    public async Task ScriptDoesNotExists_WhenUpdateContent()
    {
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var error = new ScriptDoesNotExistsError(scriptId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getScripts
            .Get(scriptId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(error));

        var result = await _updateScriptService.UpdateContent(
            new UpdateScriptContentCommand(projectId, scriptId, "content"),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateScriptContentUpdated()
    {
        // Arrange
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var script = BasicScript(projectId, scriptId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getScripts
            .Get(scriptId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(script));
        _updateFile
            .UpdateContent(projectId, script.File.Path, "content", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Success, FileDoesNotExistsError>>(new Success()));

        // Act
        var result = await _updateScriptService.UpdateContent(
            new UpdateScriptContentCommand(projectId, scriptId, "content"),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.AsT0.ShouldBe(new Success());
        await _updateFile
            .Received(1)
            .UpdateContent(
                projectId,
                script.File.Path,
                "content",
                TestContext.Current.CancellationToken
            );
    }

    #endregion
}
