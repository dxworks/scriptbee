using ScriptBee.Ports.Driving.UseCases.Project;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;

public record WebCreateProjectCommand(string Id, string Name)
{
    public CreateProjectCommand Map()
    {
        return new CreateProjectCommand(Id, Name);
    }
}
