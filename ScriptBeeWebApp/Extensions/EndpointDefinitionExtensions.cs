namespace ScriptBeeWebApp.Extensions;

public static class EndpointDefinitionExtensions
{
    public static void AddEndpointDefinitions(this IServiceCollection services)
    {
        services.AddEndpointDefinitions(typeof(IEndpointDefinition));
    }

    public static void AddEndpointDefinitions(this IServiceCollection services, params Type[] scanMarkers)
    {
        var endpointDefinitions = new List<IEndpointDefinition>();

        foreach (var scanMarker in scanMarkers)
        {
            endpointDefinitions.AddRange(
                scanMarker.Assembly.ExportedTypes
                    .Where(x => typeof(IEndpointDefinition).IsAssignableFrom(x) &&
                                x is { IsInterface: false, IsAbstract: false })
                    .Select(Activator.CreateInstance)
                    .Cast<IEndpointDefinition>());
        }

        foreach (var endpointDefinition in endpointDefinitions)
        {
            endpointDefinition.DefineServices(services);
        }

        services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
    }

    public static void UseEndpointDefinitions(this WebApplication app)
    {
        var endpointDefinitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();

        foreach (var endpointDefinition in endpointDefinitions)
        {
            endpointDefinition.DefineEndpoints(app);
        }
    }
}
