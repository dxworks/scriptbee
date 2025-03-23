using System.Threading.Channels;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Web.BackgroundServices;

public class RunScriptBackgroundService(
    Channel<RunScriptRequest> runScriptChannel,
    IRunScriptService runScriptService
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await runScriptChannel.Reader.WaitToReadAsync(stoppingToken))
        {
            var request = await runScriptChannel.Reader.ReadAsync(stoppingToken);
            await runScriptService.RunAsync(request, stoppingToken);
        }
    }
}
