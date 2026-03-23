using System.Threading.Channels;
using ScriptBee.Analysis.Web.BackgroundServices;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Service.Analysis;

namespace ScriptBee.Analysis.Web.Extensions;

public static class RunScriptExtensions
{
    public static IServiceCollection AddRunScriptServices(this IServiceCollection services)
    {
        return services
            .AddHostedService<RunScriptBackgroundService>()
            .AddSingleton<Channel<RunScriptRequest>>(_ =>
                Channel.CreateUnbounded<RunScriptRequest>(
                    new UnboundedChannelOptions { SingleReader = true }
                )
            )
            .AddSingleton<IRunScriptService, RunScriptService>();
    }
}
