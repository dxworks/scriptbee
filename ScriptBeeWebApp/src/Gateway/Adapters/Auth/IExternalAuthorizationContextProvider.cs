using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using ScriptBee.Adapters.Auth.Contracts;

namespace ScriptBee.Adapters.Auth;

public interface IExternalAuthorizationContextProvider
{
    Task<ExternalAuthorizationRequest> BuildRequestAsync(
        HttpContext httpContext,
        string action,
        CancellationToken cancellationToken
    );

    Task<ExternalAuthorizationRequest> BuildRequestAsync(
        HubInvocationContext hubInvocationContext,
        string action,
        CancellationToken cancellationToken
    );
}
