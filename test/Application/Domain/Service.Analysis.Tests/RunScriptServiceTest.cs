using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Files;
using ScriptBee.Ports.Plugins;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class RunScriptServiceTest
{
    private readonly ILoadFile _loadFile = Substitute.For<ILoadFile>();
    private readonly IUpdateAnalysis _updateAnalysis = Substitute.For<IUpdateAnalysis>();
    private readonly IFileModelService _fileModelService = Substitute.For<IFileModelService>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();
    private readonly IProjectManager _projectManager = Substitute.For<IProjectManager>();

    private readonly IScriptRunner _scriptRunner = Substitute.For<IScriptRunner>();

    private readonly RunScriptService _runScriptService;

    public RunScriptServiceTest()
    {
        _runScriptService = new RunScriptService(
            _loadFile,
            _updateAnalysis,
            _fileModelService,
            _dateTimeProvider,
            _guidProvider,
            _pluginRepository,
            _projectManager
        );
    }

    [Fact]
    public async Task RunAsync_ShouldUpdateAnalysisWithFailure_WhenFileDoesNotExist()
    {
        var request = new RunScriptRequest(_scriptRunner, CreateScript(), CreateAnalysisInfo());
        var finishedDate = DateTime.UtcNow;
        _loadFile
            .GetScriptContent(Arg.Any<ProjectId>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<string, FileDoesNotExistsError>>(
                    new FileDoesNotExistsError("path")
                )
            );
        _dateTimeProvider.UtcNow().Returns(finishedDate);

        await _runScriptService.RunAsync(request);

        await _updateAnalysis
            .Received(1)
            .Update(
                Arg.Is<AnalysisInfo>(a =>
                    a.Status == AnalysisStatus.Finished
                    && a.FinishedDate == finishedDate
                    && a.Errors.Single().Equals(new AnalysisError("File does not exist: path"))
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task RunAsync_ShouldUpdateAnalysisWithSuccess_WhenScriptRunsSuccessfully()
    {
        var script = CreateScript();
        var request = new RunScriptRequest(_scriptRunner, script, CreateAnalysisInfo());
        var finishedDate = DateTime.UtcNow;
        _loadFile
            .GetScriptContent(Arg.Any<ProjectId>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<string, FileDoesNotExistsError>>("content"));
        _guidProvider
            .NewGuid()
            .Returns(
                new Guid("00000000-0000-0000-0000-000000000001"),
                new Guid("00000000-0000-0000-0000-000000000002")
            );
        _fileModelService
            .UploadFileAsync(Arg.Any<FileId>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _dateTimeProvider.UtcNow().Returns(finishedDate);
        _pluginRepository
            .GetPlugins<IHelperFunctions>(Arg.Any<List<(Type @interface, object instance)>>())
            .Returns(new List<IHelperFunctions>());
        _projectManager.GetProject().Returns(new Project());
        _scriptRunner
            .RunAsync(
                Arg.Any<Project>(),
                Arg.Any<HelperFunctionsContainer>(),
                script.Parameters,
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.CompletedTask);

        await _runScriptService.RunAsync(request);

        await _updateAnalysis
            .Received(2)
            .Update(Arg.Any<AnalysisInfo>(), Arg.Any<CancellationToken>());
        await _updateAnalysis
            .Received(1)
            .Update(
                Arg.Is<AnalysisInfo>(a =>
                    a.Status == AnalysisStatus.Finished
                    && a.FinishedDate == finishedDate
                    && a.ScriptFileId == new FileId("00000000-0000-0000-0000-000000000001")
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task RunAsync_ShouldUpdateAnalysisWithFailure_WhenScriptThrowsException()
    {
        var script = CreateScript();
        var request = new RunScriptRequest(_scriptRunner, script, CreateAnalysisInfo());
        var finishedDate = DateTime.UtcNow;
        _loadFile
            .GetScriptContent(Arg.Any<ProjectId>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<string, FileDoesNotExistsError>>("content"));
        _guidProvider
            .NewGuid()
            .Returns(
                new Guid("00000000-0000-0000-0000-000000000001"),
                new Guid("00000000-0000-0000-0000-000000000002")
            );
        _fileModelService
            .UploadFileAsync(Arg.Any<FileId>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _dateTimeProvider.UtcNow().Returns(finishedDate);
        _pluginRepository
            .GetPlugins<IHelperFunctions>(Arg.Any<List<(Type @interface, object instance)>>())
            .Returns(new List<IHelperFunctions>());
        _projectManager.GetProject().Returns(new Project());
        _scriptRunner
            .RunAsync(
                Arg.Any<Project>(),
                Arg.Any<HelperFunctionsContainer>(),
                script.Parameters,
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Throws(new Exception("error"));

        await _runScriptService.RunAsync(request);

        await _updateAnalysis
            .Received(2)
            .Update(Arg.Any<AnalysisInfo>(), Arg.Any<CancellationToken>());
        await _updateAnalysis
            .Received(1)
            .Update(
                Arg.Is<AnalysisInfo>(a =>
                    a.Status == AnalysisStatus.Finished
                    && a.FinishedDate == finishedDate
                    && a.ScriptFileId == new FileId("00000000-0000-0000-0000-000000000001")
                ),
                Arg.Any<CancellationToken>()
            );
    }

    private static Script CreateScript()
    {
        return new Script(
            new ScriptId(),
            ProjectId.FromValue("project-id"),
            "name",
            "filePath",
            "absolute-path",
            new ScriptLanguage("language", ".lang"),
            [
                new ScriptParameter
                {
                    Name = "param",
                    Type = ScriptParameter.TypeInteger,
                    Value = 2,
                },
            ]
        );
    }

    private static AnalysisInfo CreateAnalysisInfo()
    {
        return new AnalysisInfo(
            new AnalysisId("805110c5-12e0-4bb5-a3b2-39c67dcead5b"),
            ProjectId.FromValue("project-id"),
            new ScriptId("2151737d-3d3d-41b4-802d-99519204d883"),
            null,
            AnalysisStatus.Started,
            [],
            [],
            DateTimeOffset.UtcNow,
            null
        );
    }
}
