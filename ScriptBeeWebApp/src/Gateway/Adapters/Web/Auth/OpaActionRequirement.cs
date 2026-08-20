using Microsoft.AspNetCore.Authorization;

namespace ScriptBee.Web.Auth;

public class OpaActionRequirement(string action) : IAuthorizationRequirement
{
    public string Action { get; } = action;
}
