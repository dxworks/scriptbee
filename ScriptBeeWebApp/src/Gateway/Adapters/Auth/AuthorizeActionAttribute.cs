using Microsoft.AspNetCore.Authorization;

namespace ScriptBee.Adapters.Auth;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeActionAttribute(string action)
    : AuthorizeAttribute(action),
        IAuthorizationRequirementData
{
    public string Action => Policy!;

    public IEnumerable<IAuthorizationRequirement> GetRequirements() =>
        [new PermissionActionRequirement(Action)];
}
