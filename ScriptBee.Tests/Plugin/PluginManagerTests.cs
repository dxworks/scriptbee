// todo
// using System;
// using System.Collections.Generic;
// using Moq;
// using ScriptBee.FileManagement;
// using ScriptBee.Plugin;
// using ScriptBee.Plugin.Manifest;
// using Serilog;
// using Xunit;
//
// namespace ScriptBee.Tests.Plugin;
//
// public class PluginManagerTests
// {
//     private readonly Mock<IPluginReader> _pluginManifestReaderMock;
//     private readonly Mock<IPluginRepository> _pluginRepositoryMock;
//     private readonly Mock<IPluginService> _pluginServiceMock;
//     private readonly Mock<IFileService> _fileServiceMock;
//
//     private readonly PluginManager _pluginManager;
//
//     public PluginManagerTests()
//     {
//         var loggerMock = new Mock<ILogger>();
//
//         _pluginManifestReaderMock = new Mock<IPluginReader>();
//         _pluginRepositoryMock = new Mock<IPluginRepository>();
//         _pluginServiceMock = new Mock<IPluginService>();
//         _fileServiceMock = new Mock<IFileService>();
//
//         _pluginManager = new PluginManager(loggerMock.Object, _pluginManifestReaderMock.Object,
//             _pluginRepositoryMock.Object, _fileServiceMock.Object, _pluginServiceMock.Object);
//     }
//
//     [Fact]
//     public void GivenEmptyManifest_WhenLoadPlugins_ThenNoPluginIsLoaded()
//     {
//         _pluginManifestReaderMock.Setup(x => x.ReadPlugins(It.IsAny<string>()))
//             .Returns(new List<(string path, PluginManifest)>());
//
//         _pluginManager.LoadPlugins("path");
//     }
// }
