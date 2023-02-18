using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ScriptBeeWebApp.Tests.Contract.ProviderStates;

namespace ScriptBeeWebApp.Tests.Contract.Middleware;

public class ProviderStateMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ProviderStateLocator _providerStateLocator;
    
    public ProviderStateMiddleware(RequestDelegate next, ProviderStateLocator providerStateLocator)
    {
        _next = next;
        _providerStateLocator = providerStateLocator;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.Value == "/provider-states")
        {
            HandleProviderStatesRequest(context);
            await context.Response.WriteAsync(string.Empty);
        }
        else
        {
            await _next(context);
        }
    }

    private void HandleProviderStatesRequest(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        if (!string.Equals(context.Request.Method, HttpMethod.Post.ToString(),
                StringComparison.CurrentCultureIgnoreCase) || context.Request.Body is null)
        {
            return;
        }

        string jsonRequestBody;
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
        {
            jsonRequestBody = reader.ReadToEnd();
        }

        var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

        if (providerState != null && !string.IsNullOrEmpty(providerState.State))
        {
            _providerStateLocator.LocateStateAction(providerState.State).Invoke();
        }
    }
}
