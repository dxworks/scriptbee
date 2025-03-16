using Refit;

namespace ScriptBee.Rest.Api;

public interface IContextApi
{
    [Post("/api/context/clear")]
    Task Clear(CancellationToken cancellationToken);
}
