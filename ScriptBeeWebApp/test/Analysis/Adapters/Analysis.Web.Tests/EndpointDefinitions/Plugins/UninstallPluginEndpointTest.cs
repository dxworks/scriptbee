// using System.Net;
// using Microsoft.Extensions.DependencyInjection;
// using NSubstitute;
// using OneOf;
// using OneOf.Types;
// using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;
// using ScriptBee.Tests.Common;
// using ScriptBee.UseCases.Plugin;
//
// namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Plugins;
//
// public class UninstallPluginEndpointTest(ITestOutputHelper outputHelper)
// {
//     private const string TestUrl = "/api/plugins";
//
//     [Fact]
//     public async Task ShouldUninstallPlugin_WithValidParameters()
//     {
//         var pluginId = "test-plugin";
//         var version = "1.0.0";
//
//         var useCase = Substitute.For<IUninstallPluginUseCase>();
//         useCase.UninstallPlugin(pluginId, version).Returns(new Success());
//
//         var url = $"{TestUrl}/{pluginId}?version={version}";
//         var api = new TestApiCaller<Program>(url);
//
//         var response = await api.DeleteApi(
//             new TestWebApplicationFactory<Program>(
//                 outputHelper,
//                 services =>
//                 {
//                     services.AddSingleton(useCase);
//                 }
//             )
//         );
//
//         response.StatusCode.ShouldBe(HttpStatusCode.OK);
//         var content = await response.ReadContentAsync<WebUninstallPluginResponse>();
//         content.PluginId.ShouldBe(pluginId);
//         content.Version.ShouldBe(version);
//         useCase.Received(1).UninstallPlugin(pluginId, version);
//     }
//
//     [Fact]
//     public async Task ShouldCallUninstallUseCase_WithCorrectParameters()
//     {
//         var pluginId = "my-plugin";
//         var version = "2.0.0";
//
//         var useCase = Substitute.For<IUninstallPluginUseCase>();
//         useCase.UninstallPlugin(pluginId, version).Returns(new Success());
//
//         var url = $"{TestUrl}/{pluginId}?version={version}";
//         var api = new TestApiCaller<Program>(url);
//
//         var response = await api.DeleteApi(
//             new TestWebApplicationFactory<Program>(
//                 outputHelper,
//                 services =>
//                 {
//                     services.AddSingleton(useCase);
//                 }
//             )
//         );
//
//         response.StatusCode.ShouldBe(HttpStatusCode.OK);
//         useCase.Received(1).UninstallPlugin(pluginId, version);
//     }
//
//     [Fact]
//     public async Task ShouldReturnBadRequest_WhenUninstallationFails()
//     {
//         var pluginId = "test-plugin";
//         var version = "1.0.0";
//
//         var error = new OneOf.Types.Error { Message = "Plugin not found" };
//         var useCase = Substitute.For<IUninstallPluginUseCase>();
//         useCase.UninstallPlugin(pluginId, version).Returns(error);
//
//         var url = $"{TestUrl}/{pluginId}?version={version}";
//         var api = new TestApiCaller<Program>(url);
//
//         var response = await api.DeleteApi(
//             new TestWebApplicationFactory<Program>(
//                 outputHelper,
//                 services =>
//                 {
//                     services.AddSingleton(useCase);
//                 }
//             )
//         );
//
//         response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
//     }
//
//     [Fact]
//     public async Task ShouldReturnOk_WhenPluginUninstalledSuccessfully()
//     {
//         var pluginId = "test-plugin";
//         var version = "1.0.0";
//
//         var useCase = Substitute.For<IUninstallPluginUseCase>();
//         useCase.UninstallPlugin(pluginId, version).Returns(new Success());
//
//         var url = $"{TestUrl}/{pluginId}?version={version}";
//         var api = new TestApiCaller<Program>(url);
//
//         var response = await api.DeleteApi(
//             new TestWebApplicationFactory<Program>(
//                 outputHelper,
//                 services =>
//                 {
//                     services.AddSingleton(useCase);
//                 }
//             )
//         );
//
//         response.StatusCode.ShouldBe(HttpStatusCode.OK);
//     }
// }
