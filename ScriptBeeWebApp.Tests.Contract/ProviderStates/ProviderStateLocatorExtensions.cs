using Microsoft.Extensions.DependencyInjection;

namespace ScriptBeeWebApp.Tests.Contract.ProviderStates;

public static class ProviderStateLocatorExtensions
{
    public static void RegisterProviderStateLocatorMocks(this IServiceCollection services)
    {
        var scanMarker = typeof(IProviderStateDefinition);

        var providerStateDefinitions = scanMarker.Assembly.ExportedTypes
            .Where(x => typeof(IProviderStateDefinition).IsAssignableFrom(x) &&
                        x is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IProviderStateDefinition>()
            .ToList();

        foreach (var endpointDefinition in providerStateDefinitions)
        {
            endpointDefinition.RegisterMocks(services);
        }

        services.AddSingleton<ProviderStateLocator>(_ => new ProviderStateLocator(providerStateDefinitions));
    }
}
