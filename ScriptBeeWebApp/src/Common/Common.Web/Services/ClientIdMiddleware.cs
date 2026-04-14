using Microsoft.AspNetCore.Http;
using ScriptBee.Application.Model.Services;

namespace ScriptBee.Common.Web.Services;

public class ClientIdMiddleware(RequestDelegate next)
{
    private const string ClientIdHeaderName = "X-Client-Id";

    public async Task InvokeAsync(HttpContext context, IClientIdProvider clientIdProvider)
    {
        if (context.Request.Headers.TryGetValue(ClientIdHeaderName, out var clientId))
        {
            clientIdProvider.ClientId = clientId;
        }

        await next(context);
    }
}
