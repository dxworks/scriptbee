﻿using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.Services;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Python;

public class PythonScriptRunner : IScriptRunner
{
    public string Language => "python";

    public async Task RunAsync(IProject project, IHelperFunctionsContainer helperFunctionsContainer,
        IEnumerable<ScriptParameter> parameters, string scriptContent, CancellationToken cancellationToken = default)
    {
        var pythonEngine = IronPython.Hosting.Python.CreateEngine();

        var dictionary = new Dictionary<string, object>
        {
            {
                "project", project
            },
        };

        foreach (var (functionName, delegateFunction) in helperFunctionsContainer.GetFunctionsDictionary())
        {
            dictionary.Add(functionName, delegateFunction);
        }

        var scriptScope = pythonEngine.CreateScope(dictionary);

        var validScript = new ScriptGeneratorStrategy().ExtractValidScript(scriptContent);

        await Task.Run(() => { pythonEngine.Execute(validScript, scriptScope); }, cancellationToken);
    }
}
