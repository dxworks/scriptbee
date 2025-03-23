using System.Threading.Channels;
using DxWorks.ScriptBee.Plugin.Api;
using NSubstitute;
using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Analysis;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Project.Structure;
using ScriptBee.Service.Analysis;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Service.Tests;

public class RunAnalysisServiceTest
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly ICreateAnalysis _createAnalysis = Substitute.For<ICreateAnalysis>();
    private readonly IGetScript _getScript = Substitute.For<IGetScript>();
    private readonly IPluginRepository _pluginRepository = Substitute.For<IPluginRepository>();

    private readonly Channel<RunScriptRequest> _runScriptChannel =
        Channel.CreateUnbounded<RunScriptRequest>();

    private readonly IScriptRunner _scriptRunner = Substitute.For<IScriptRunner>();

    private readonly RunAnalysisService _runAnalysisService;

    public RunAnalysisServiceTest()
    {
        _runAnalysisService = new RunAnalysisService(
            _dateTimeProvider,
            _guidProvider,
            _createAnalysis,
            _getScript,
            _pluginRepository,
            _runScriptChannel
        );
    }

    [Fact]
    public async Task CreateAnalysisSuccessful()
    {
        var creationDate = DateTimeOffset.UtcNow;
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId(Guid.NewGuid());
        var scriptId = new ScriptId(Guid.NewGuid());
        var command = new RunAnalysisCommand(projectId, scriptId);
        var expectedAnalysisInfo = new AnalysisInfo(
            analysisId,
            projectId,
            scriptId,
            null,
            AnalysisStatus.Started,
            [],
            [],
            creationDate,
            null
        );
        var script = new Script(
            scriptId,
            projectId,
            "script",
            "path",
            "absolute-path",
            new ScriptLanguage("language", ".lang"),
            []
        );
        _dateTimeProvider.UtcNow().Returns(creationDate);
        _guidProvider.NewGuid().Returns(analysisId.Value);
        _getScript
            .Get(scriptId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(script));
        _createAnalysis
            .Create(
                Arg.Is<AnalysisInfo>(info => info.MatchAnalysisResult(expectedAnalysisInfo)),
                Arg.Any<CancellationToken>()
            )
            .Returns(expectedAnalysisInfo);
        _pluginRepository.GetPlugin(Arg.Any<Func<IScriptRunner, bool>>()).Returns(_scriptRunner);

        var analysisResult = await _runAnalysisService.Run(command);

        analysisResult.AssertAnalysisResult(expectedAnalysisInfo);
        var runScriptRequest = await _runScriptChannel.Reader.ReadAsync();

        runScriptRequest.AnalysisInfo.ShouldBe(expectedAnalysisInfo);
        runScriptRequest.ScriptRunner.ShouldBe(_scriptRunner);
        runScriptRequest.Script.ShouldBe(script);
    }

    [Fact]
    public async Task GivenScriptRunnerDoesNotExistsError_thenAnalysisIsFailed()
    {
        var date = DateTimeOffset.UtcNow;
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId(Guid.Parse("97c3498a-1a8f-4e01-be79-0cc9ede9f7a8"));
        var scriptId = new ScriptId(Guid.Parse("38900b32-fca2-4213-bd4b-c71cc1068bb6"));
        var command = new RunAnalysisCommand(projectId, scriptId);
        var expectedAnalysisInfo = new AnalysisInfo(
            analysisId,
            projectId,
            scriptId,
            null,
            AnalysisStatus.Finished,
            [],
            [new AnalysisError("Runner for language 'language' does not exist.")],
            date,
            date
        );
        _dateTimeProvider.UtcNow().Returns(date);
        _guidProvider.NewGuid().Returns(analysisId.Value);
        _getScript
            .Get(scriptId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(
                    new Script(
                        scriptId,
                        projectId,
                        "script",
                        "path",
                        "absolute-path",
                        new ScriptLanguage("language", ".lang"),
                        []
                    )
                )
            );
        _createAnalysis
            .Create(
                Arg.Is<AnalysisInfo>(info => info.MatchAnalysisResult(expectedAnalysisInfo)),
                Arg.Any<CancellationToken>()
            )
            .Returns(expectedAnalysisInfo);
        _pluginRepository
            .GetPlugin(Arg.Any<Func<IScriptRunner, bool>>())
            .Returns((IScriptRunner?)null);

        var analysisResult = await _runAnalysisService.Run(command);

        analysisResult.AssertAnalysisResult(expectedAnalysisInfo);
    }

    [Fact]
    public async Task GivenScriptDoesNotExistsError_thenAnalysisIsFailed()
    {
        var date = DateTimeOffset.UtcNow;
        var projectId = ProjectId.FromValue("project-id");
        var analysisId = new AnalysisId(Guid.Parse("647b7d62-0c2f-48d5-9aa2-bded959de486"));
        var scriptId = new ScriptId(Guid.Parse("ec253791-9b1b-4a35-9ccb-6d82720ba461"));
        var command = new RunAnalysisCommand(projectId, scriptId);
        var expectedAnalysisInfo = new AnalysisInfo(
            analysisId,
            projectId,
            scriptId,
            null,
            AnalysisStatus.Finished,
            [],
            [new AnalysisError($"Script '{scriptId}' does not exist.")],
            date,
            date
        );
        _dateTimeProvider.UtcNow().Returns(date);
        _guidProvider.NewGuid().Returns(analysisId.Value);
        _getScript
            .Get(scriptId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Script, ScriptDoesNotExistsError>>(
                    new ScriptDoesNotExistsError(scriptId)
                )
            );
        _createAnalysis
            .Create(
                Arg.Is<AnalysisInfo>(info => info.MatchAnalysisResult(expectedAnalysisInfo)),
                Arg.Any<CancellationToken>()
            )
            .Returns(expectedAnalysisInfo);

        var analysisResult = await _runAnalysisService.Run(command);

        analysisResult.AssertAnalysisResult(expectedAnalysisInfo);
    }
}

public static class AnalysisAssertionsExtensions
{
    public static void AssertAnalysisResult(this AnalysisInfo actual, AnalysisInfo expected)
    {
        actual.Id.ShouldBeEquivalentTo(expected.Id);
        actual.ProjectId.ShouldBe(expected.ProjectId);
        actual.ScriptId.ShouldBe(expected.ScriptId);
        actual.Status.ShouldBe(expected.Status);
        actual.Results.ToList().ShouldBe(expected.Results.ToList());
        actual.Errors.ToList().ShouldBe(expected.Errors.ToList());
        actual.CreationDate.ShouldBe(expected.CreationDate);
        actual.FinishedDate.ShouldBe(expected.FinishedDate);
    }

    public static bool MatchAnalysisResult(this AnalysisInfo actual, AnalysisInfo expected)
    {
        return actual.Id.Equals(expected.Id)
            && actual.ProjectId.Equals(expected.ProjectId)
            && actual.ScriptId.Equals(expected.ScriptId)
            && actual.Status.Equals(expected.Status)
            && actual.Results.SequenceEqual(expected.Results)
            && actual.Errors.SequenceEqual(expected.Errors)
            && actual.CreationDate.Equals(expected.CreationDate)
            && actual.FinishedDate.Equals(expected.FinishedDate);
    }
}
