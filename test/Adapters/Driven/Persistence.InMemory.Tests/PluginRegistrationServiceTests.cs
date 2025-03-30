namespace ScriptBee.Persistence.InMemory.Tests;

public class PluginRegistrationServiceTests
{
    private readonly PluginRegistrationService _pluginRegistrationService = new();

    [Fact]
    public void GivenPluginKindAndAcceptedTypes_WhenAdd_ThenValuesAreRegistered()
    {
        var acceptedTypes = new HashSet<Type> { typeof(string), typeof(PluginRegistrationService) };

        _pluginRegistrationService.Add("kind", acceptedTypes);

        Assert.True(_pluginRegistrationService.TryGetValue("kind", out var registeredTypes));
        Assert.Equal(acceptedTypes, registeredTypes);
    }

    [Fact]
    public void GivenExistingPluginKind_WhenAdd_ThenPreviousValuesIsOverriden()
    {
        var acceptedTypes = new HashSet<Type> { typeof(object) };

        _pluginRegistrationService.Add("kind", []);
        _pluginRegistrationService.Add("kind", acceptedTypes);

        Assert.True(_pluginRegistrationService.TryGetValue("kind", out var registeredTypes));
        Assert.Equal(acceptedTypes, registeredTypes);
    }
}
