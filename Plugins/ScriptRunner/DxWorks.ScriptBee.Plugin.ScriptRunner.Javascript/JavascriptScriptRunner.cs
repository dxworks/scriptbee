using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using Jint;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript;

public class JavascriptScriptRunner : IScriptRunner
{
    private readonly IHelperFunctionsFactory _helperFunctionsFactory;
    private readonly IScriptGeneratorStrategy _scriptGeneratorStrategy;

    public JavascriptScriptRunner(IHelperFunctionsFactory helperFunctionsFactory,
        IScriptGeneratorStrategy scriptGeneratorStrategy)
    {
        _helperFunctionsFactory = helperFunctionsFactory;
        _scriptGeneratorStrategy = scriptGeneratorStrategy;
    }

    public string Language => "javascript";

    public async Task RunAsync(IProject project, string runId, string scriptContent,
        CancellationToken cancellationToken = default)
    {
        var engine = new Engine();
        engine.SetValue("project", project);

        var helperFunctionsContainer = _helperFunctionsFactory.Create(project.Id, runId);

        foreach (var (functionName, delegateFunction) in helperFunctionsContainer.GetFunctionsDictionary())
        {
            engine.SetValue(functionName, delegateFunction);
        }

        var validScript = _scriptGeneratorStrategy.ExtractValidScript(scriptContent);

        await Task.Run(() => { engine.Execute(validScript); }, cancellationToken);
    }
}
