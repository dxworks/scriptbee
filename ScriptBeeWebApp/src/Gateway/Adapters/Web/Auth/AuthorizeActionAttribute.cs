using Microsoft.AspNetCore.Authorization;

namespace ScriptBee.Web.Auth;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeActionAttribute(string action)
    : AuthorizeAttribute,
        IAuthorizationRequirementData
{
    public IEnumerable<IAuthorizationRequirement> GetRequirements() =>
        [new OpaActionRequirement(action)];
}
