using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;
using Jint;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript;

public class JavascriptScriptRunner : IScriptRunner
{
    public string Language => "javascript";

    public async Task RunAsync(IProject project, IHelperFunctionsContainer helperFunctionsContainer,
        string scriptContent, CancellationToken cancellationToken = default)
    {
        var engine = new Engine();
        engine.SetValue("project", project);

        foreach (var (functionName, delegateFunction) in helperFunctionsContainer.GetFunctionsDictionary())
        {
            engine.SetValue(functionName, delegateFunction);
        }

        await Task.Run(() => { engine.Execute(scriptContent); }, cancellationToken);
    }
}
