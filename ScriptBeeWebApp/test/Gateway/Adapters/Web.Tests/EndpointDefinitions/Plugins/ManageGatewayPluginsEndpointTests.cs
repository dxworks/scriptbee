using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Tests.Common;
using ScriptBee.Tests.Common.Plugins;
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
                new List<Plugin>
                {
                    new TestPlugin(new PluginId("plugin1", new Version(1, 0, 0))),
                    new TestPlugin(new PluginId("plugin2", new Version(2, 0, 0))),
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

    [Theory]
    [FilePath("TestData/ManageGatewayPlugins/ui-plugins-response.json")]
    public async Task GivenUiPlugins_WhenGetGatewayPluginsWithKindUI_ThenReturnOnlyUiPlugins(
        string responsePath
    )
    {
        var useCase = Substitute.For<IManagePluginsUseCase>();

        var uiPlugin = new TestUiPlugin(
            new PluginId("scriptbee-ui-plugin-example", new Version(1, 0, 0))
        );

        useCase
            .GetInstalledPlugins()
            .Returns(
                new List<Plugin>
                {
                    new TestPlugin(new PluginId("plugin1", new Version(1, 0, 0))),
                    uiPlugin,
                }
            );
        useCase.GetInstalledPlugins().Returns(new List<Plugin> { uiPlugin });

        TestApiCaller<Program> api = new($"{TestUrl}?kind=UI");
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

    [Theory]
    [FilePath("TestData/ManageGatewayPlugins/ui-manifest-response.json")]
    public async Task GivenUiPlugins_WhenGetUiPluginsManifest_ThenReturnCorrectManifestMap(
        string responsePath
    )
    {
        var useCase = Substitute.For<IManagePluginsUseCase>();
        useCase
            .GetUiPluginsManifest()
            .Returns(
                new Dictionary<string, string>
                {
                    { "scriptbee-ui-plugin-example", "http://localhost:4201/remoteEntry.json" },
                }
            );

        TestApiCaller<Program> api = new("/api/plugins/gateway/ui/manifest");
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
    public async Task GivenValidFilePath_WhenServeUiPluginFile_ThenReturnFile()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.js");
        await File.WriteAllTextAsync(tempFile, "test content");

        var useCase = Substitute.For<IManagePluginsUseCase>();
        useCase
            .GetUiPluginFilePath(new PluginId("id", new Version("1.0.0")), "test.js")
            .Returns(tempFile);

        TestApiCaller<Program> api = new("/api/plugins/gateway/ui/files/id/1.0.0/test.js");
        try
        {
            var response = await api.GetApi(
                new TestWebApplicationFactory<Program>(
                    outputHelper,
                    services =>
                    {
                        services.AddSingleton(useCase);
                    }
                )
            );

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.ShouldBe("test content");
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task GivenInvalidFilePath_WhenServeUiPluginFile_ThenReturnNotFound()
    {
        var useCase = Substitute.For<IManagePluginsUseCase>();
        useCase
            .GetUiPluginFilePath(new PluginId("id", new Version("1.0.0")), "invalid.js")
            .Returns((string?)null);

        TestApiCaller<Program> api = new("/api/plugins/gateway/ui/files/id/1.0.0/invalid.js");
        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
