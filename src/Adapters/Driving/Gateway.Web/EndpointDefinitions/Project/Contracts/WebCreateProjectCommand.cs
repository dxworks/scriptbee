using ScriptBee.Ports.Driving.UseCases.Projects;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;

public record WebCreateProjectCommand(string Name)
{
    public CreateProjectCommand Map()
    {
        return new CreateProjectCommand(Name);
    }
}
