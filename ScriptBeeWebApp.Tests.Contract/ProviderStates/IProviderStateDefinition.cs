using Microsoft.Extensions.DependencyInjection;

namespace ScriptBeeWebApp.Tests.Contract.ProviderStates;

public interface IProviderStateDefinition
{
    void RegisterMocks(IServiceCollection serviceCollection);
}
