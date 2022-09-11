﻿using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScriptBee.Plugin.Manifest;
using ScriptBeeWebApp.Controllers;
using ScriptBeeWebApp.Services;
using Xunit;

namespace ScriptBeeWebApp.Tests.Unit.Controllers;

public class PluginControllerTests
{
    private readonly Mock<IPluginService> _pluginServiceMock;
    private readonly Fixture _fixture;

    private readonly PluginController _pluginController;

    public PluginControllerTests()
    {
        _pluginServiceMock = new Mock<IPluginService>();

        _pluginController = new PluginController(_pluginServiceMock.Object);

        _fixture = new Fixture();
    }

    [Fact]
    public void GivenPlugins_WhenGetLoadedPlugins_ThenListOfPluginsIsReturned()
    {
        _pluginServiceMock.Setup(x => x.GetLoadedPlugins()).Returns(new List<PluginManifest>
        {
            _fixture.Create<ScriptGeneratorPluginManifest>(),
            _fixture.Create<UiPluginManifest>(),
            _fixture.Create<ScriptGeneratorPluginManifest>(),
        });

        var actionResult = _pluginController.GetLoadedPlugins();
        var plugins = (OkObjectResult)actionResult.Result!;
        var pluginResponses = (IEnumerable<PluginManifest>)plugins.Value!;

        Assert.Equal(3, pluginResponses.Count());
    }
}
