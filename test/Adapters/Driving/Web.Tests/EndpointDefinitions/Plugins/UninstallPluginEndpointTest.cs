using System.Net;
using ScriptBee.Tests.Common;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Plugins;

public class UninstallPluginEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/plugins/plugin-id";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnOk()
    {
        var response = await _api.DeleteApi(new TestWebApplicationFactory<Program>(outputHelper));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
