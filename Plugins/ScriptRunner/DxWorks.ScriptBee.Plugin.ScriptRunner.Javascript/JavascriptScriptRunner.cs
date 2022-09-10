using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using DxWorks.ScriptBee.Plugin.Api.ScriptRunner;
using Jint;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript;

public class JavascriptScriptRunner : IScriptRunner
{
    private readonly IHelperFunctionsMapper _helperFunctionsMapper;
    private readonly IHelperFunctionsFactory _helperFunctionsFactory;
    private readonly IScriptGeneratorStrategy _scriptGeneratorStrategy;

    public JavascriptScriptRunner(IHelperFunctionsMapper helperFunctionsMapper,
        IHelperFunctionsFactory helperFunctionsFactory, IScriptGeneratorStrategy scriptGeneratorStrategy)
    {
        _helperFunctionsMapper = helperFunctionsMapper;
        _helperFunctionsFactory = helperFunctionsFactory;
        _scriptGeneratorStrategy = scriptGeneratorStrategy;
    }

    public async Task RunAsync(IProject project, string runId, string scriptContent,
        CancellationToken cancellationToken = default)
    {
        var engine = new Engine();
        engine.SetValue("project", project);

        var helperFunctions = _helperFunctionsFactory.Create(project.Id, runId);

        foreach (var (functionName, delegateFunction) in _helperFunctionsMapper.GetFunctionsDictionary(
                     helperFunctions))
        {
            engine.SetValue(functionName, delegateFunction);
        }

        var validScript = _scriptGeneratorStrategy.ExtractValidScript(scriptContent);

        await Task.Run(() => { engine.Execute(validScript); }, cancellationToken);
    }
}
