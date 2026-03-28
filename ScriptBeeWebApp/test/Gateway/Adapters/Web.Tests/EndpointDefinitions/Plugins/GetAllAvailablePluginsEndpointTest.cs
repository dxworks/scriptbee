using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Plugin;
using ScriptBee.Domain.Model.Plugin.MarketPlace;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Plugin;
using VeriJson;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Plugins;

public class GetAllAvailablePluginsEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/plugins";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetAllAvailablePlugins/response.json")]
    public async Task ShouldReturnPluginsList(string responsePath)
    {
        var useCase = Substitute.For<IGetAvailablePluginsUseCase>();
        useCase
            .GetMarketPlugins(Arg.Any<CancellationToken>())
            .Returns([
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
                ),
            ]);

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var actualContent = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        var expectedContent = await File.ReadAllTextAsync(
            FilePathAttribute.GetFilePath(responsePath),
            TestContext.Current.CancellationToken
        );
        actualContent.Should().BeEquivalentTo(expectedContent);
    }
}
