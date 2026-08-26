using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Permissions;

public class GetGlobalPermissionsEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/permissions";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetGlobalPermissions/response.json")]
    public async Task ShouldReturnGlobalPermissions(string responsePath)
    {
        // Arrange
        var useCase = Substitute.For<IGlobalPermissionsUseCase>();
        useCase
            .GetGlobalPermissions(
                Arg.Any<GetGlobalPermissionsQuery>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<List<string>>(["project:create", "gateway_plugin:management"])
            );

        // Act
        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetGlobalPermissions/empty_response.json")]
    public async Task GivenNoPermissions_ShouldReturnEmptyPermissions(string responsePath)
    {
        // Arrange
        var useCase = Substitute.For<IGlobalPermissionsUseCase>();
        useCase
            .GetGlobalPermissions(
                Arg.Any<GetGlobalPermissionsQuery>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<List<string>>([]));

        // Act
        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }
}
