using System.ComponentModel;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

[Description("Command used to create a new project toke.")]
public record WebCreateProjectTokenRequest(
    string? Description,
    string Role,
    DateTimeOffset ExpiresAt
)
{
    public CreateProjectTokenCommand Map(ProjectId projectId)
    {
        return new CreateProjectTokenCommand(projectId, Description, new UserRole(Role), ExpiresAt);
    }
}
