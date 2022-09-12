namespace DxWorks.ScriptBee.Plugin.Api;

public interface IHelperFunctionsFactory
{
    public IHelperFunctionsContainer Create(string projectId, string runId);
}
