using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Plugins.MarketPlace;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Plugins;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Plugins;

public class GetAvailablePluginByIdEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/plugins/plugin-id";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetAvailablePluginById/response.json")]
    public async Task ShouldReturnPluginById(string responsePath)
    {
        var useCase = Substitute.For<IGetAvailablePluginsUseCase>();
        useCase
            .GetMarketPlugin("plugin-id", Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<MarketPlacePlugin, PluginNotFoundError>>(
                    new MarketPlacePlugin(
                        "plugin1",
                        "Plugin 1",
                        MarketPlacePluginType.Plugin,
                        "Description for Plugin 1",
                        ["Author 1", "Author 2"],
                        [
                            new PluginVersion(
                                "https://example.com/plugin1/v1.0.0/download",
                                new Version("1.0.0"),
                                "https://example.com/plugin1/v1.0.0/download"
                            ),
                        ]
                    )
                )
            );

        var response = await _api.GetApi(
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
    public async Task ShouldReturnNotFound_WhenPluginNotFoundErrorIsReturned()
    {
        var useCase = Substitute.For<IGetAvailablePluginsUseCase>();
        useCase
            .GetMarketPlugin("plugin-id", Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<MarketPlacePlugin, PluginNotFoundError>>(
                    new PluginNotFoundError(new PluginId("plugin-id", new Version()))
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(
            response.Content,
            TestUrl,
            "Plugin Not Found",
            "A plugin with the ID 'plugin-id' does not exists."
        );
    }
}
