using ScriptBee.ProjectContext;

namespace ScriptBee.Scripts.ScriptRunners;

public interface IScriptRunner
{
    void Run(Project project, string scriptContent);
}