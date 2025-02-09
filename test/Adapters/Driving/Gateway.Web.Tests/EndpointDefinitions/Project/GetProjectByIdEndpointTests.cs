using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Authorization;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;
using ScriptBee.Ports.Driving.UseCases.Project;
using ScriptBee.Tests.Common;
using Shouldly;
using Xunit.Abstractions;
using static ScriptBee.Gateway.Web.Tests.ProblemValidationUtils;

namespace ScriptBee.Gateway.Web.Tests.EndpointDefinitions.Project;

public class GetProjectByIdEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id";
    private readonly TestApiCaller _api = new(TestUrl);

    public static TheoryData<string> ProvideUserRoleTypes =
        [UserRole.Guest.Type, UserRole.Analyst.Type, UserRole.Maintainer.Type, UserRole.Administrator.Type];

    [Theory]
    [MemberData(nameof(ProvideUserRoleTypes))]
    public async Task GivenRole_ShouldReturnProjectDetails(string roleType)
    {
        var role = UserRole.FromType(roleType);
        var projectId = ProjectId.FromValue("id");
        var query = new GetProjectQuery(projectId);
        var getProjectsUseCase = Substitute.For<IGetProjectsUseCase>();
        var creationDate = DateTime.Parse("2024-02-08");
        getProjectsUseCase.GetProject(query, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                new ProjectDetails(projectId, "name", creationDate)));

        var response = await _api.GetApi(new TestWebApplicationFactory<Program>(outputHelper, [role],
            services => { services.AddSingleton(getProjectsUseCase); }));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var getResponse = await response.ReadContentAsync<WebGetProjectDetailsResponse>();
        getResponse.ShouldBe(new WebGetProjectDetailsResponse("id", "name", creationDate));
    }

    [Theory]
    [MemberData(nameof(ProvideUserRoleTypes))]
    public async Task GivenRoleAndProjectNotExists_ShouldReturnNotFound(string roleType)
    {
        var role = UserRole.FromType(roleType);
        var projectId = ProjectId.FromValue("id");
        var query = new GetProjectQuery(projectId);
        var getProjectsUseCase = Substitute.For<IGetProjectsUseCase>();
        getProjectsUseCase.GetProject(query, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                new ProjectDoesNotExistsError(projectId)));

        var response = await _api.GetApi(new TestWebApplicationFactory<Program>(outputHelper, [role],
            services => { services.AddSingleton(getProjectsUseCase); }));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertNotFoundProblem(response.Content, TestUrl,
            "Project Not Found",
            "A project with the ID 'id' does not exists."
        );
    }

    [Fact]
    public async Task NoRoles_ShouldReturnForbidden()
    {
        var response =
            await _api.GetApi(new TestWebApplicationFactory<Program>(outputHelper, []));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UnauthorizedUser_ShouldReturnUnauthorized()
    {
        var response = await _api.GetApiWithoutAuthorization(new TestWebApplicationFactory<Program>(outputHelper));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
