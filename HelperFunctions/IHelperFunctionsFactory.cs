namespace HelperFunctions;

public interface IHelperFunctionsFactory
{
    public IHelperFunctionsWithResults Create(string projectId, string runId);
}