// todo
// using System.Collections.Generic;
// using System.Linq;
// using AutoFixture;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using ScriptBee.Plugin;
// using ScriptBee.Plugin.Manifest;
// using ScriptBeeWebApp.Controllers;
// using ScriptBeeWebApp.Services;
// using Xunit;
//
// namespace ScriptBeeWebApp.Tests.Unit.Controllers;
// todo to be replaced by Pact tests
// public class PluginControllerTests
// {
//     private readonly Mock<IPluginRepository> _pluginRepositoryMock;
//     private readonly Fixture _fixture;
//
//     private readonly PluginController _pluginController;
//
//     public PluginControllerTests()
//     {
//         _pluginRepositoryMock = new Mock<IPluginRepository>();
//
//         _pluginController = new PluginController(_pluginRepositoryMock.Object);
//
//         _fixture = new Fixture();
//     }
//
//     [Fact]
//     public void GivenPlugins_WhenGetLoadedPlugins_ThenListOfPluginsIsReturned()
//     {
//         _pluginRepositoryMock.Setup(x => x.GetLoadedPlugins()).Returns(new List<PluginManifest>
//         {
//             _fixture.Create<ScriptGeneratorPluginManifest>(),
//             _fixture.Create<UiPluginManifest>(),
//             _fixture.Create<ScriptGeneratorPluginManifest>(),
//         });
//
//         var actionResult = _pluginController.GetLoadedPlugins();
//         var plugins = (OkObjectResult)actionResult.Result!;
//         var pluginResponses = (IEnumerable<PluginManifest>)plugins.Value!;
//
//         Assert.Equal(3, pluginResponses.Count());
//     }
//
//     [Fact]
//     public void GivenPluginType_WhenGetLoadedPlugins_ThenListOfPluginsIsReturned()
//     {
//         _pluginRepositoryMock.Setup(x => x.GetLoadedPlugins()).Returns(new List<PluginManifest>
//         {
//             _fixture.Create<ScriptGeneratorPluginManifest>(),
//             _fixture.Build<UiPluginManifest>()
//                 .With(m => m.Kind, PluginTypes.Ui)
//                 .Create(),
//             _fixture.Create<ScriptGeneratorPluginManifest>(),
//             _fixture.Build<UiPluginManifest>()
//                 .With(m => m.Kind, PluginTypes.Ui)
//                 .Create(),
//             _fixture.Create<ScriptGeneratorPluginManifest>(),
//         });
//
//         var actionResult = _pluginController.GetLoadedPlugins(PluginTypes.Ui);
//         var plugins = (OkObjectResult)actionResult.Result!;
//         var pluginResponses = (IEnumerable<PluginManifest>)plugins.Value!;
//
//         Assert.Equal(2, pluginResponses.Count());
//     }
// }


