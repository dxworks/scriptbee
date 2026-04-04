using DxWorks.ScriptBee.Plugin.Api.Model;
using NSubstitute;
using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Plugin;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Project.Tests.ProjectStructure;

public class CreateScriptServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly ICreateFile _createFile = Substitute.For<ICreateFile>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly ICreateScript _createScript = Substitute.For<ICreateScript>();

    private readonly CreateScriptService _createScriptService;

    public CreateScriptServiceTest()
    {
        _createScriptService = new CreateScriptService(
            _getProject,
            _createFile,
            _guidProvider,
            _createScript,
            new ScriptGeneratorStrategyFactory()
        );
    }

    [Fact]
    public async Task ProjectDoesNotExists()
    {
        var projectId = ProjectId.FromValue("id");
        var error = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(error));

        var result = await _createScriptService.Create(
            new CreateScriptCommand(
                projectId,
                "path",
                "language",
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
    public async Task ScriptLanguageDoesNotExists()
    {
        var projectId = ProjectId.FromValue("id");
        var error = new ScriptLanguageDoesNotExistsError("language");
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );

        var result = await _createScriptService.Create(
            new CreateScriptCommand(
                projectId,
                "path",
                "language",
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
    public async Task CreateFileAlreadyExists()
    {
        var projectId = ProjectId.FromValue("id");
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _createFile
            .Create(projectId, "path.cs", Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectStructureFile, FileAlreadyExistsError>>(
                    new FileAlreadyExistsError("path.cs")
                )
            );

        var result = await _createScriptService.Create(
            new CreateScriptCommand(
                projectId,
                "path",
                "csharp",
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

        result.ShouldBe(new ScriptPathAlreadyExistsError("path.cs"));
    }

    [Fact]
    public async Task CreateScriptSuccessfully()
    {
        var projectId = ProjectId.FromValue("id");
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _createFile
            .Create(projectId, "path.cs", Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectStructureFile, FileAlreadyExistsError>>(
                    new ProjectStructureFile("path.cs")
                )
            );
        _guidProvider.NewGuid().Returns(Guid.Parse("3554c7c6-0fd8-4556-af5b-c23d92ea05b1"));

        var result = await _createScriptService.Create(
            new CreateScriptCommand(
                projectId,
                "path.cs",
                "csharp",
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

        var script = new Script(
            new ScriptId("3554c7c6-0fd8-4556-af5b-c23d92ea05b1"),
            projectId,
            new ProjectStructureFile("path.cs"),
            new ScriptLanguage("csharp", ".cs"),
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = "string",
                    Value = "value",
                },
            ]
        );
        MatchScript(result.AsT0, script);
        await _createScript
            .Received(1)
            .Create(
                Arg.Is<Script>(x => MatchScript(x, script)),
                TestContext.Current.CancellationToken
            );
    }

    [Fact]
    public async Task CreateScriptSuccessfullyWithExtension()
    {
        var projectId = ProjectId.FromValue("id");
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _createFile
            .Create(projectId, "path.ext.cs", Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectStructureFile, FileAlreadyExistsError>>(
                    new ProjectStructureFile("path.ext.cs")
                )
            );
        _guidProvider.NewGuid().Returns(Guid.Parse("3554c7c6-0fd8-4556-af5b-c23d92ea05b1"));

        var result = await _createScriptService.Create(
            new CreateScriptCommand(
                projectId,
                "path.ext",
                "csharp",
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

        var script = new Script(
            new ScriptId("3554c7c6-0fd8-4556-af5b-c23d92ea05b1"),
            projectId,
            new ProjectStructureFile("path.ext.cs"),
            new ScriptLanguage("csharp", ".cs"),
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = "string",
                    Value = "value",
                },
            ]
        );
        MatchScript(result.AsT0, script);
        await _createScript
            .Received(1)
            .Create(
                Arg.Is<Script>(x => MatchScript(x, script)),
                TestContext.Current.CancellationToken
            );
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
}
