using DxWorks.ScriptBee.Plugin.Api.Model;
using DxWorks.ScriptBee.Plugin.Api.ScriptGeneration;
using DxWorks.ScriptBee.Plugin.Api.ScriptRunner;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.Python;

public class PythonScriptRunner : IScriptRunner
{
    private readonly IHelperFunctionsFactory _helperFunctionsFactory;
    private readonly IHelperFunctionsMapper _helperFunctionsMapper;
    private readonly IScriptGeneratorStrategy _scriptGeneratorStrategy;

    public PythonScriptRunner(IHelperFunctionsFactory helperFunctionsFactory,
        IHelperFunctionsMapper helperFunctionsMapper, IScriptGeneratorStrategy scriptGeneratorStrategy)
    {
        _helperFunctionsFactory = helperFunctionsFactory;
        _helperFunctionsMapper = helperFunctionsMapper;
        _scriptGeneratorStrategy = scriptGeneratorStrategy;
    }

    public async Task RunAsync(IProject project, string runId, string scriptContent,
        CancellationToken cancellationToken = default)
    {
        var pythonEngine = IronPython.Hosting.Python.CreateEngine();

        var helperFunctions = _helperFunctionsFactory.Create(project.Id, runId);

        var dictionary = new Dictionary<string, object>
        {
            {
                "project", project
            },
        };

        foreach (var (functionName, delegateFunction) in _helperFunctionsMapper.GetFunctionsDictionary(helperFunctions))
        {
            dictionary.Add(functionName, delegateFunction);
        }

        var scriptScope = pythonEngine.CreateScope(dictionary);

        var validScript = _scriptGeneratorStrategy.ExtractValidScript(scriptContent);

        await Task.Run(() => { pythonEngine.Execute(validScript, scriptScope); }, cancellationToken);
    }
}
