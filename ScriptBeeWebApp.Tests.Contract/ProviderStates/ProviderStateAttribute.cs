namespace ScriptBeeWebApp.Tests.Contract.ProviderStates;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ProviderStateAttribute : Attribute
{
    public string State { get; }

    public ProviderStateAttribute(string state)
    {
        State = state;
    }
}
