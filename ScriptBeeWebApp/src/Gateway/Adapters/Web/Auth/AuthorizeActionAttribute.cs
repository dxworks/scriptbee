using Microsoft.AspNetCore.Authorization;

namespace ScriptBee.Web.Auth;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeActionAttribute(string action)
    : AuthorizeAttribute,
        IAuthorizationRequirementData
{
    public string Action => action;

    public IEnumerable<IAuthorizationRequirement> GetRequirements() =>
        [new PermissionActionRequirement(action)];
}
