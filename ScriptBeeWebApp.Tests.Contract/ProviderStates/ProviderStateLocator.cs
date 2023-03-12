using System.Reflection;

namespace ScriptBeeWebApp.Tests.Contract.ProviderStates;

public class ProviderStateLocator
{
    private readonly Dictionary<string, Action> _providerActions;

    public ProviderStateLocator(IEnumerable<IProviderStateDefinition> providerStateDefinitions)
    {
        _providerActions = new Dictionary<string, Action>();


        RegisterProviderStateDefinition(providerStateDefinitions);
    }

    public Action LocateStateAction(string state)
    {
        return _providerActions[state];
    }

    private void RegisterProviderStateDefinition(IEnumerable<IProviderStateDefinition> providerStateDefinitions)
    {
        foreach (var providerStateDefinition in providerStateDefinitions)
        {
            foreach (var methodInfo in providerStateDefinition.GetType().GetMethods())
            {
                var attribute = methodInfo.GetCustomAttribute<ProviderStateAttribute>();
                if (attribute is null)
                {
                    continue;
                }

                var action = methodInfo.CreateDelegate<Action>(providerStateDefinition);

                _providerActions.Add(attribute.State, action);
            }
        }
    }
}
