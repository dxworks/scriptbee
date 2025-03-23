namespace DxWorks.ScriptBee.Plugin.Api.Services;

public interface IHelperFunctionsContainer
{
    public Dictionary<string, Delegate> GetFunctionsDictionary();

    public IEnumerable<IHelperFunctions> GetFunctions();
}
