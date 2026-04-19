using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Gateway.ProjectStructure;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebCreateScriptCommand(
    string Path,
    string Language,
    IEnumerable<WebScriptParameter>? Parameters
)
{
    public CreateScriptCommand Map(ProjectId projectId)
    {
        return new CreateScriptCommand(
            projectId,
            Path,
            Language,
            (Parameters ?? []).Select(p => p.Map())
        );
    }
}
