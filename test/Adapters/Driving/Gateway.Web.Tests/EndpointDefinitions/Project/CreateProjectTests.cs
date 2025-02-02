using System.Net;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests.EndpointDefinitions.Project;

public class CreateProjectTests(ITestOutputHelper outputHelper)
{
    private readonly TestApiCaller<CreateProject.WebCreateProjectCommand> _api = new("/api/scriptbee/projects",
        outputHelper);

    [Fact]
    public async Task MaintainerRole_ShouldReturnCreated()
    {
        var response = await _api.PostApi(["MAINTAINER"]);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OtherRole_ShouldReturnForbidden()
    {
        var response = await _api.PostApi(["OTHER"]);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task NoRoles_ShouldReturnForbidden()
    {
        var response = await _api.PostApi([]);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UnauthorizedUser_ShouldReturnUnauthorized()
    {
        var response = await _api.PostApiWithoutAuthorization();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
