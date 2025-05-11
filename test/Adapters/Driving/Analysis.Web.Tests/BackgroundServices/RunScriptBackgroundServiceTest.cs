using System.Threading.Channels;
using DxWorks.ScriptBee.Plugin.Api;
using NSubstitute;
using ScriptBee.Analysis.Web.BackgroundServices;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Web.Tests.BackgroundServices;

public class RunScriptBackgroundServiceTest
{
    private readonly IScriptRunner _scriptRunner = Substitute.For<IScriptRunner>();

    [Fact]
    public async Task ExecuteAsync_ShouldProcessRequestsFromChannel()
    {
        var runScriptChannel = Channel.CreateUnbounded<RunScriptRequest>();
        var runScriptService = Substitute.For<IRunScriptService>();
        var backgroundService = new RunScriptBackgroundService(runScriptChannel, runScriptService);
        var request1 = new RunScriptRequest(
            _scriptRunner,
            CreateScript(),
            CreateAnalysisInfo("b4e4954c-dd83-4cc8-bc52-bd52a6eed515")
        );
        var request2 = new RunScriptRequest(
            _scriptRunner,
            CreateScript(),
            CreateAnalysisInfo("5ba00f63-1b3b-48a2-8abf-73489ddb3bec")
        );
        await runScriptChannel.Writer.WriteAsync(request1, TestContext.Current.CancellationToken);
        await runScriptChannel.Writer.WriteAsync(request2, TestContext.Current.CancellationToken);
        runScriptChannel.Writer.Complete();

        await backgroundService.StartAsync(CancellationToken.None);
        await backgroundService.StopAsync(CancellationToken.None);

        await runScriptService.Received(1).RunAsync(request1, Arg.Any<CancellationToken>());
        await runScriptService.Received(1).RunAsync(request2, Arg.Any<CancellationToken>());
    }

    private static Script CreateScript()
    {
        return new Script(
            new ScriptId("2151737d-3d3d-41b4-802d-99519204d883"),
            ProjectId.FromValue("project-id"),
            "name",
            "filePath",
            "absolute-path",
            new ScriptLanguage("language", ".lang"),
            []
        );
    }

    private static AnalysisInfo CreateAnalysisInfo(string analysisId)
    {
        return new AnalysisInfo(
            new AnalysisId(analysisId),
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
