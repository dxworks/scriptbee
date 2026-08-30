using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.User;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.EndpointDefinitions.ProjectAccess.Contracts;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectAccess;

public class ManageProjectAccessEndpointTests(ITestOutputHelper outputHelper)
{
    private const string ProjectId = "project-id";

    [Theory]
    [FilePath("TestData/GetProjectMembers/response.json")]
    public async Task GetProjectMembers_ShouldReturnMembers(string responsePath)
    {
        var api = new TestApiCaller<Program>($"/api/projects/{ProjectId}/members");
        var projectId = Domain.Model.Project.ProjectId.FromValue(ProjectId);
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .GetProjectMembers(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new List<ProjectMember>
                    {
                        new("user-a", "user", new UserRole("owner")),
                        new("team-b", "group", new UserRole("viewer")),
                    }
                )
            );

        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetProjectMembers/empty_response.json")]
    public async Task GetProjectMembers_WhenNoMembers_ShouldReturnEmptyList(string responsePath)
    {
        var api = new TestApiCaller<Program>($"/api/projects/{ProjectId}/members");
        var projectId = Domain.Model.Project.ProjectId.FromValue(ProjectId);
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .GetProjectMembers(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<ProjectMember>()));

        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Fact]
    public async Task UpdateProjectMember_ShouldReturnNoContent()
    {
        var api = new TestApiCaller<Program>($"/api/projects/{ProjectId}/members/user-a");
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .UpdateProjectMember(
                Arg.Any<UpdateProjectMemberCommand>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.CompletedTask);

        var response = await api.PutApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new WebUpdateProjectMemberCommand("editor", "user")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await useCase
            .Received(1)
            .UpdateProjectMember(
                Arg.Is<UpdateProjectMemberCommand>(c =>
                    c.MemberId == "user-a"
                    && c.MemberType == "user"
                    && c.Role == new UserRole("editor")
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task UpdateProjectMember_InvalidBody_ShouldReturnBadRequest()
    {
        var api = new TestApiCaller<Program>($"/api/projects/{ProjectId}/members/user-a");

        var response = await api.PutApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebUpdateProjectMemberCommand("", "user")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveProjectMember_ShouldReturnNoContent()
    {
        var api = new TestApiCaller<Program>(
            $"/api/projects/{ProjectId}/members/user-a?memberType=user"
        );
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .RemoveProjectMember(
                Arg.Any<RemoveProjectMemberCommand>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.CompletedTask);

        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await useCase
            .Received(1)
            .RemoveProjectMember(
                Arg.Is<RemoveProjectMemberCommand>(c =>
                    c.MemberId == "user-a" && c.MemberType == "user"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Theory]
    [FilePath("TestData/GetAllUsers/response.json")]
    public async Task GetAllUsers_ShouldReturnUserList(string responsePath)
    {
        var api = new TestApiCaller<Program>("/api/users");
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .GetAllUsers(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<UserInfo> { new(new UserId("id-1"), "Alice") }));

        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }

    [Theory]
    [FilePath("TestData/GetAvailableRoles/response.json")]
    public async Task GetAvailableRoles_ShouldReturnRoleList(string responsePath)
    {
        var api = new TestApiCaller<Program>("/api/roles");
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .GetAvailableRoles(Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    new List<RoleInfo> { new("owner", "Full control over the project") }
                )
            );

        var response = await api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }
}
