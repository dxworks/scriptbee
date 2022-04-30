using System.Collections.Generic;
using System.Threading.Tasks;
using HelperFunctions;
using ScriptBee.ProjectContext;

namespace ScriptBee.Scripts.ScriptRunners;

public interface IScriptRunner
{
    public Task<List<RunResult>> Run(Project project, string runId, string scriptContent);
}