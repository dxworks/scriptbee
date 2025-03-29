using DxWorks.ScriptBee.Plugin.Api.Model;
using NSubstitute;
using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Files;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Project;
using ScriptBee.Ports.Project.Structure;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.UseCases.Project.ProjectStructure;
using static ScriptBee.Tests.Common.InstanceInfoFixture;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Project.Tests.ProjectStructure;

public class CreateScriptServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly IGetScriptLanguages _getScriptLanguages =
        Substitute.For<IGetScriptLanguages>();

    private readonly ICreateFile _createFile = Substitute.For<ICreateFile>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly ICreateScript _createScript = Substitute.For<ICreateScript>();

    private readonly IGetCurrentInstanceUseCase _getCurrentInstanceUseCase =
        Substitute.For<IGetCurrentInstanceUseCase>();

    private readonly CreateScriptService _createScriptService;

    public CreateScriptServiceTest()
    {
        _createScriptService = new CreateScriptService(
            _getProject,
            _getScriptLanguages,
            _createFile,
            _guidProvider,
            _createScript,
            _getCurrentInstanceUseCase
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
            )
        );

        result.ShouldBe(error);
    }

    [Fact]
    public async Task ScriptLanguageDoesNotExists()
    {
        var projectId = ProjectId.FromValue("id");
        var error = new ScriptLanguageDoesNotExistsError("language");
        var instanceInfo = BasicInstanceInfo(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getCurrentInstanceUseCase
            .GetOrAllocate(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(instanceInfo));
        _getScriptLanguages
            .Get(instanceInfo, "language", Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ScriptLanguage, ScriptLanguageDoesNotExistsError>>(error)
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
            )
        );

        result.ShouldBe(error);
    }

    [Fact]
    public async Task CreateFileAlreadyExists()
    {
        var projectId = ProjectId.FromValue("id");
        var instanceInfo = BasicInstanceInfo(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getCurrentInstanceUseCase
            .GetOrAllocate(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(instanceInfo));
        _getScriptLanguages
            .Get(instanceInfo, "language", Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ScriptLanguage, ScriptLanguageDoesNotExistsError>>(
                    new ScriptLanguage("language", ".lang")
                )
            );
        _createFile
            .Create(projectId, "path.lang", "", Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<CreateFileResult, FileAlreadyExistsError>>(
                    new FileAlreadyExistsError("path.lang")
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
            )
        );

        result.ShouldBe(new ScriptPathAlreadyExistsError("path.lang"));
    }

    [Fact]
    public async Task CreateScriptSuccessfully()
    {
        var projectId = ProjectId.FromValue("id");
        var instanceInfo = BasicInstanceInfo(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getCurrentInstanceUseCase
            .GetOrAllocate(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(instanceInfo));
        _getScriptLanguages
            .Get(instanceInfo, "language", Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ScriptLanguage, ScriptLanguageDoesNotExistsError>>(
                    new ScriptLanguage("language", ".lang")
                )
            );
        _createFile
            .Create(projectId, "path.lang", "", Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<CreateFileResult, FileAlreadyExistsError>>(
                    new CreateFileResult("name", "path.lang", "absolute-path.lang")
                )
            );
        _guidProvider.NewGuid().Returns(Guid.Parse("3554c7c6-0fd8-4556-af5b-c23d92ea05b1"));

        var result = await _createScriptService.Create(
            new CreateScriptCommand(
                projectId,
                "path.lang",
                "language",
                [
                    new ScriptParameter
                    {
                        Name = "parameter",
                        Type = "string",
                        Value = "value",
                    },
                ]
            )
        );

        var script = new Script(
            new ScriptId("3554c7c6-0fd8-4556-af5b-c23d92ea05b1"),
            projectId,
            "name",
            "path.lang",
            "absolute-path.lang",
            new ScriptLanguage("language", ".lang"),
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
        await _createScript.Received(1).Create(Arg.Is<Script>(x => MatchScript(x, script)));
    }

    [Fact]
    public async Task CreateScriptSuccessfullyWithExtension()
    {
        var projectId = ProjectId.FromValue("id");
        var instanceInfo = BasicInstanceInfo(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _getCurrentInstanceUseCase
            .GetOrAllocate(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(instanceInfo));
        _getScriptLanguages
            .Get(instanceInfo, "language", Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ScriptLanguage, ScriptLanguageDoesNotExistsError>>(
                    new ScriptLanguage("language", ".lang")
                )
            );
        _createFile
            .Create(projectId, "path.ext.lang", "", Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<CreateFileResult, FileAlreadyExistsError>>(
                    new CreateFileResult("name", "path.ext.lang", "absolute-path.ext.lang")
                )
            );
        _guidProvider.NewGuid().Returns(Guid.Parse("3554c7c6-0fd8-4556-af5b-c23d92ea05b1"));

        var result = await _createScriptService.Create(
            new CreateScriptCommand(
                projectId,
                "path.ext",
                "language",
                [
                    new ScriptParameter
                    {
                        Name = "parameter",
                        Type = "string",
                        Value = "value",
                    },
                ]
            )
        );

        var script = new Script(
            new ScriptId("3554c7c6-0fd8-4556-af5b-c23d92ea05b1"),
            projectId,
            "name",
            "path.ext.lang",
            "absolute-path.ext.lang",
            new ScriptLanguage("language", ".lang"),
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
        await _createScript.Received(1).Create(Arg.Is<Script>(x => MatchScript(x, script)));
    }

    private static bool MatchScript(Script actual, Script expected)
    {
        return actual.Id.Equals(expected.Id)
            && actual.ProjectId.Equals(expected.ProjectId)
            && actual.Name.Equals(expected.Name)
            && actual.FilePath.Equals(expected.FilePath)
            && actual.AbsoluteFilePath.Equals(expected.AbsoluteFilePath)
            && actual.ScriptLanguage.Equals(expected.ScriptLanguage)
            && MatchParameter(actual.Parameters);
    }

    private static bool MatchParameter(IEnumerable<ScriptParameter> parameters)
    {
        return parameters.Single() is { Name: "parameter", Type: "string", Value: "value" };
    }
}
