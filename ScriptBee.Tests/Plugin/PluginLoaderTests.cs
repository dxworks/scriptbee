﻿using System;
using System.Collections.Generic;
using Moq;
using ScriptBee.FileManagement;
using ScriptBee.Plugin;
using ScriptBee.Tests.Plugin.Internals;
using Serilog;
using Xunit;

namespace ScriptBee.Tests.Plugin;

public class PluginLoaderTests
{
    private readonly Mock<IDllLoader> _dllLoaderMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<ILogger> _loggerMock;

    private readonly PluginLoader _pluginLoader;
    private readonly Mock<IPluginRegistrationService> _pluginRegistrationServiceMock;
    private readonly Mock<IPluginRepository> _pluginRepositoryMock;

    public PluginLoaderTests()
    {
        _loggerMock = new Mock<ILogger>();
        _fileServiceMock = new Mock<IFileService>();
        _dllLoaderMock = new Mock<IDllLoader>();
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _pluginRegistrationServiceMock = new Mock<IPluginRegistrationService>();

        _pluginLoader = new PluginLoader(_loggerMock.Object, _fileServiceMock.Object, _dllLoaderMock.Object,
            _pluginRepositoryMock.Object, _pluginRegistrationServiceMock.Object);
    }

    [Fact]
    public void GivenUnregisteredPlugin_WhenLoad_ThenMessageIsLogged()
    {
        HashSet<Type>? nullTypes = null;
        var plugin = new TestPlugin("id", new Version(0, 0, 0, 1));

        _pluginRegistrationServiceMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out nullTypes))
            .Returns(false);

        _pluginLoader.Load(plugin);

        _loggerMock.Verify(l =>
            l.Warning("Plugin kind {PluginKind} has no relevant Dlls to load",
                plugin.Manifest.ExtensionPoints[0].Kind));
    }

    [Fact]
    public void GivenRegisteredPluginWithNoAcceptedTypes_WhenLoad_ThenPluginManifestIsLoaded()
    {
        var acceptedTypes = new HashSet<Type>();
        var plugin = new TestPlugin("id", new Version(0, 0, 0, 1));

        _pluginRegistrationServiceMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out acceptedTypes))
            .Returns(true);
        _fileServiceMock.Setup(s => s.CombinePaths(plugin.FolderPath, plugin.Manifest.ExtensionPoints[0].EntryPoint))
            .Returns("entrypoint.dll");
        _dllLoaderMock.Setup(l => l.LoadDllTypes("entrypoint.dll", acceptedTypes))
            .Returns(new List<(Type @interface, Type concrete)>());

        _pluginLoader.Load(plugin);

        _pluginRepositoryMock.Verify(r => r.RegisterPlugin(plugin),
            Times.Once());
    }

    [Fact]
    public void GivenRegisteredPluginWithMultipleAcceptedTypes_WhenLoad_ThenPluginManifestIsLoaded()
    {
        var acceptedTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(object)
        };
        var plugin = new TestPlugin("id", new Version(0, 0, 0, 1));

        _pluginRegistrationServiceMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out acceptedTypes))
            .Returns(true);
        _fileServiceMock.Setup(s => s.CombinePaths(plugin.FolderPath, plugin.Manifest.ExtensionPoints[0].EntryPoint))
            .Returns("entrypoint.dll");
        _dllLoaderMock.Setup(l => l.LoadDllTypes("entrypoint.dll", acceptedTypes))
            .Returns(new List<(Type @interface, Type concrete)>
            {
                (typeof(string), typeof(string)),
                (typeof(object), typeof(object))
            });

        _pluginLoader.Load(plugin);

        _pluginRepositoryMock.Verify(r => r.RegisterPlugin(plugin, typeof(object), typeof(object)), Times.Once());
    }
}
