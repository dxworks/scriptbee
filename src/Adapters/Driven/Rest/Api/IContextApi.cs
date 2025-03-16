using Refit;
using ScriptBee.Rest.Contracts;

namespace ScriptBee.Rest.Api;

public interface IContextApi
{
    [Post("/api/context/clear")]
    Task Clear(CancellationToken cancellationToken);

    [Post("/api/context/link")]
    Task Link([Body] RestContextLink request, CancellationToken cancellationToken);
}
