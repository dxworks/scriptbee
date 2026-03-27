using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Plugin;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Plugins;

public class UninstallPluginEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/plugins/test-plugin";

    [Fact]
    public async Task ShouldUninstallPlugin_WithValidParameters()
    {
        const string pluginId = "test-plugin";
        const string version = "1.0.0";

        var useCase = Substitute.For<IUninstallPluginUseCase>();

        const string url = $"{TestUrl}?version={version}";
        var api = new TestApiCaller<Program>(url);
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        useCase.Received(1).UninstallPlugin(pluginId, version);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenVersionIsNotProvided()
    {
        var useCase = Substitute.For<IUninstallPluginUseCase>();

        var api = new TestApiCaller<Program>(TestUrl);
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
