using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Service.Analysis;

public interface IRunScriptService
{
    Task RunAsync(RunScriptRequest request, CancellationToken cancellationToken = default);
}
