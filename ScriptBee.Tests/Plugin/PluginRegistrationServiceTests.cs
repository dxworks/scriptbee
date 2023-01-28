using System;
using System.Collections.Generic;
using ScriptBee.Plugin;
using Xunit;

namespace ScriptBee.Tests.Plugin;

public class PluginRegistrationServiceTests
{
    private readonly PluginRegistrationService _pluginRegistrationService;

    public PluginRegistrationServiceTests()
    {
        _pluginRegistrationService = new PluginRegistrationService();
    }

    [Fact]
    public void GivenPluginKindAndAcceptedTypes_WhenAdd_ThenValuesAreRegistered()
    {
        var acceptedTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(PluginRegistrationService)
        };

        _pluginRegistrationService.Add("kind", acceptedTypes);

        Assert.True(_pluginRegistrationService.TryGetValue("kind", out var registeredTypes));
        Assert.Equal(acceptedTypes, registeredTypes);
    }

    [Fact]
    public void GivenExistingPluginKind_WhenAdd_ThenPreviousValuesIsOverriden()
    {
        var acceptedTypes = new HashSet<Type>
        {
            typeof(object),
        };

        _pluginRegistrationService.Add("kind", new HashSet<Type>());
        _pluginRegistrationService.Add("kind", acceptedTypes);

        Assert.True(_pluginRegistrationService.TryGetValue("kind", out var registeredTypes));
        Assert.Equal(acceptedTypes, registeredTypes);
    }
}
