using Microsoft.AspNetCore.Authentication;

namespace ScriptBee.Adapters.Auth;

public class ProjectTokenAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string Scheme = "ProjectToken";
}
