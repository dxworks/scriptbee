using System.Dynamic;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;
using Jint;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript;

public class JavascriptScriptRunner : IScriptRunner
{
    public string Language => "javascript";

    public async Task RunAsync(IProject project, IHelperFunctionsContainer helperFunctionsContainer,
        IEnumerable<ScriptParameter> parameters, string scriptContent, CancellationToken cancellationToken = default)
    {
        var engine = new Engine();
        engine.SetValue("project", project);
        engine.SetValue("scriptParameters", CreateScriptParameters(parameters));

        foreach (var (functionName, delegateFunction) in helperFunctionsContainer.GetFunctionsDictionary())
        {
            engine.SetValue(functionName, delegateFunction);
        }

        var validScript = new ScriptGeneratorStrategy().ExtractValidScript(scriptContent);

        await Task.Run(() => { engine.Execute(validScript); }, cancellationToken);
    }

    private static dynamic CreateScriptParameters(IEnumerable<ScriptParameter> parameters)
    {
        dynamic scriptParameters = new ExpandoObject();
        
        IDictionary<string, object?> underlyingDictionary = scriptParameters;

        foreach (var parameter in parameters)
        {
            underlyingDictionary.Add(parameter.Name, parameter.Value);
        }
        
        return scriptParameters;
    }
}
