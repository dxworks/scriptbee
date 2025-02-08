namespace ScriptBee.Gateway.Web.Extensions;

public static class SwaggerEndpointExtensions
{
    public static void MapSwaggerUi(this WebApplication app)
    {
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/openapi/v1.json", "ScriptBee API"); });
    }
}
