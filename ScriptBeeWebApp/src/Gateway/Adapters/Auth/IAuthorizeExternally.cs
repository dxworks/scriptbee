using ScriptBee.Adapters.Auth.Contracts;

namespace ScriptBee.Adapters.Auth;

public interface IAuthorizeExternally
{
    public Task<bool> IsAllowed(
        ExternalAuthorizationRequest request,
        CancellationToken cancellationToken
    );
}
