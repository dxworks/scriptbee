using System.Net;
using ScriptBee.Tests.Common;
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
        var response = await _api.GetApi(new TestWebApplicationFactory<Program>(outputHelper));

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
