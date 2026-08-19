using System.Net;
using ScriptBee.Tests.Common;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Config;

public class AuthConfigEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/config";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetAuthConfig/response.json")]
    public async Task ShouldReturnOk(string responsePath)
    {
        var response = await _api.GetApi(new TestWebApplicationFactory<Program>(outputHelper));

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }
}
