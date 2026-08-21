using Microsoft.AspNetCore.Authorization;

namespace ScriptBee.Web.Auth;

public class PermissionActionRequirement(string action) : IAuthorizationRequirement
{
    public string Action { get; } = action;
}
