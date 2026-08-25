using Microsoft.AspNetCore.Authorization;

namespace ScriptBee.Adapters.Auth;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeActionAttribute(string action)
    : AuthorizeAttribute,
        IAuthorizationRequirementData
{
    public string Action => action;

    public IEnumerable<IAuthorizationRequirement> GetRequirements() =>
        [new PermissionActionRequirement(action)];
}
