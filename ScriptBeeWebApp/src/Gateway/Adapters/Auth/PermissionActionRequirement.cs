using Microsoft.AspNetCore.Authorization;

namespace ScriptBee.Adapters.Auth;

public class PermissionActionRequirement(string action) : IAuthorizationRequirement
{
    public string Action { get; } = action;
}
