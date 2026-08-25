using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Web.Auth.Dev;

public sealed class GetDevProjectPermissions(IEnumerable<EndpointDataSource> endpointDataSources)
    : IGetProjectPermissions
{
    public Task<List<string>> GetPermissions(
        ProjectId projectId,
        UserId userId,
        List<UserGroup> groups,
        UserRole userRole,
        CancellationToken cancellationToken
    )
    {
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        permissions.Add("project:edit");

        foreach (var source in endpointDataSources)
        {
            foreach (var endpoint in source.Endpoints)
            {
                foreach (var metadata in endpoint.Metadata)
                {
                    if (metadata is AuthorizeActionAttribute attribute)
                    {
                        permissions.Add(attribute.Action);
                    }
                }
            }
        }

        return Task.FromResult(permissions.ToList());
    }
}
