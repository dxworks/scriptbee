using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Python;

public class PythonScriptRunner : IScriptRunner
{
    private readonly IHelperFunctionsFactory _helperFunctionsFactory;
    private readonly IScriptGeneratorStrategy _scriptGeneratorStrategy;

    public PythonScriptRunner(IHelperFunctionsFactory helperFunctionsFactory,
        IScriptGeneratorStrategy scriptGeneratorStrategy)
    {
        _helperFunctionsFactory = helperFunctionsFactory;
        _scriptGeneratorStrategy = scriptGeneratorStrategy;
    }

    public string Language => "python";

    public async Task RunAsync(IProject project, string runId, string scriptContent,
        CancellationToken cancellationToken = default)
    {
        var pythonEngine = IronPython.Hosting.Python.CreateEngine();

        var helperFunctionsContainer = _helperFunctionsFactory.Create(project.Id, runId);

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

        var validScript = _scriptGeneratorStrategy.ExtractValidScript(scriptContent);

        await Task.Run(() => { pythonEngine.Execute(validScript, scriptScope); }, cancellationToken);
    }
}
