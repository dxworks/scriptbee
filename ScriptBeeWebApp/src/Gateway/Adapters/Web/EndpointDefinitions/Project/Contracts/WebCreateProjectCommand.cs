using System.ComponentModel;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

[Description("Command used to create a new project.")]
public record WebCreateProjectCommand(string Id, string Name)
{
    public CreateProjectCommand Map()
    {
        return new CreateProjectCommand(Id, Name);
    }
}
