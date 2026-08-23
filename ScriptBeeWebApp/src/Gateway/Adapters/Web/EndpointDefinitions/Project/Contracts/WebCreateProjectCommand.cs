using System.ComponentModel;
using ScriptBee.Domain.Model.User;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Web.EndpointDefinitions.Project.Contracts;

[Description("Command used to create a new project.")]
public record WebCreateProjectCommand(string Id, string Name)
{
    public CreateProjectCommand Map(UserId userId)
    {
        return new CreateProjectCommand(Id, Name, userId);
    }
}
