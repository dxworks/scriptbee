using Microsoft.AspNetCore.Builder;

namespace ScriptBee.Common.Web.Extensions;

public static class SwaggerEndpointExtensions
{
    public static void MapSwaggerUi(this WebApplication app)
    {
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/openapi/v1.json", "ScriptBee API"); });
    }
}
