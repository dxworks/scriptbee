using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Plugins;
using ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Plugins;

public class ManageGatewayPluginsEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/plugins/gateway";

    [Theory]
    [FilePath("TestData/ManageGatewayPlugins/response.json")]
    public async Task GivenInstalledPlugins_WhenGetGatewayPlugins_ThenReturnCorrectResponse(
        string responsePath
    )
    {
        var useCase = Substitute.For<IManagePluginsUseCase>();
        useCase
            .GetInstalledPlugins()
            .Returns(
                new List<PluginId>
                {
                    new("plugin1", new Version(1, 0, 0)),
                    new("plugin2", new Version(2, 0, 0)),
                }
            );

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Fact]
    public async Task GivenValidRequest_WhenInstallGatewayPlugin_ThenReturnNoContent()
    {
        var useCase = Substitute.For<IManagePluginsUseCase>();
        var request = new WebInstallGatewayPluginRequest("plugin1", "1.0.0");

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            request
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await useCase
            .Received(1)
            .Install(new PluginId("plugin1", new Version(1, 0, 0)), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenValidId_WhenUninstallGatewayPlugin_ThenReturnNoContent()
    {
        var useCase = Substitute.For<IManagePluginsUseCase>();
        const string pluginId = "plugin1";
        const string version = "1.0.0";

        TestApiCaller<Program> api = new($"{TestUrl}/{pluginId}?version={version}");
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
        useCase.Received(1).Uninstall(new PluginId("plugin1", new Version(1, 0, 0)));
    }
}
