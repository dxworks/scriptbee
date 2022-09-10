using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;

namespace DxWorks.ScriptBee.Plugin.Api.ScriptRunner;

public interface IHelperFunctionsFactory
{
    public IHelperFunctions Create(string projectId, string runId);
}
