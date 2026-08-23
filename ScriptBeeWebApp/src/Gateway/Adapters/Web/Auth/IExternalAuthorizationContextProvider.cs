using ScriptBee.Web.Auth.Contracts;

namespace ScriptBee.Web.Auth;

public interface IExternalAuthorizationContextProvider
{
    public Task<ExternalAuthorizationRequest> BuildRequestAsync(
        HttpContext httpContext,
        string action,
        CancellationToken cancellationToken
    );
}
