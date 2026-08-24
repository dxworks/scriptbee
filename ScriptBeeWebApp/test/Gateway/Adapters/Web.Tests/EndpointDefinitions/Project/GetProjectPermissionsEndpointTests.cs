using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Project;

public class GetProjectPermissionsEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id/permissions";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Theory]
    [FilePath("TestData/GetProjectPermissions/response.json")]
    public async Task ShouldReturnProjectPermissions(string responsePath)
    {
        var projectId = ProjectId.FromValue("id");
        var useCase = Substitute.For<IProjectPermissionsUseCase>();
        useCase
            .GetProjectPermissions(
                Arg.Is<GetProjectPermissionsQuery>(q => q.ProjectId == projectId),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<UserPermissions?>(
                    new UserPermissions(new UserRole("editor"), ["permission-1"])
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

    [Theory]
    [FilePath("TestData/GetProjectPermissions/empty_response.json")]
    public async Task GivenNoPermissions_ShouldReturnNoProjectPermissions(string responsePath)
    {
        var projectId = ProjectId.FromValue("id");
        var useCase = Substitute.For<IProjectPermissionsUseCase>();
        useCase
            .GetProjectPermissions(
                Arg.Is<GetProjectPermissionsQuery>(q => q.ProjectId == projectId),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<UserPermissions?>(null));

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
}
