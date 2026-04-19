using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

public record WebCreateProjectCommand(string Id, string Name)
{
    public CreateProjectCommand Map()
    {
        return new CreateProjectCommand(Id, Name);
    }
}
