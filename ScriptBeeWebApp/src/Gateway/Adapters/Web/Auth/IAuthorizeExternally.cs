namespace ScriptBee.Web.Auth;

public interface IAuthorizeExternally
{
    public Task<bool> IsAllowed(
        ExternalAuthorizationRequest request,
        CancellationToken cancellationToken
    );
}
