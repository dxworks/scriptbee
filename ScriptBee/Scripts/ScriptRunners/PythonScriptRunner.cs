using System.Collections.Generic;
using System.Threading.Tasks;
using HelperFunctions;
using IronPython.Hosting;
using ScriptBee.ProjectContext;
using ScriptBee.Utils.ValidScriptExtractors;
using ScriptBeeWebApp.Services;

namespace ScriptBee.Scripts.ScriptRunners;

public class PythonScriptRunner : IScriptRunner
{
    private readonly IHelperFunctionsFactory _helperFunctionsFactory;
    private readonly ValidScriptExtractor _scriptExtractor;
    private readonly IHelperFunctionsMapper _helperFunctionsMapper;

    public PythonScriptRunner(ValidScriptExtractor scriptExtractor, IHelperFunctionsFactory helperFunctionsFactory,
        IHelperFunctionsMapper helperFunctionsMapper)
    {
        _scriptExtractor = scriptExtractor;
        _helperFunctionsFactory = helperFunctionsFactory;
        _helperFunctionsMapper = helperFunctionsMapper;
    }

    public Task<List<RunResult>> Run(Project project, string runId, string scriptContent)
    {
        var pythonEngine = Python.CreateEngine();

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

        var validScript = _scriptExtractor.ExtractValidScript(scriptContent);
        pythonEngine.Execute(validScript, scriptScope);

        return helperFunctions.GetResults();
    }
}