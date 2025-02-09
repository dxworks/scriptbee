using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Authorization;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;
using ScriptBee.Ports.Driving.UseCases.Project;
using ScriptBee.Tests.Common;
using Shouldly;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests.EndpointDefinitions.Project;

public class GetAllProjectsEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects";
    private readonly TestApiCaller _api = new(TestUrl);

    public static TheoryData<string> ProvideUserRoleTypes =
        [UserRole.Guest.Type, UserRole.Analyst.Type, UserRole.Maintainer.Type, UserRole.Administrator.Type];

    [Theory]
    [MemberData(nameof(ProvideUserRoleTypes))]
    public async Task GivenRole_ShouldReturnProjectDetailsList(string roleType)
    {
        var role = UserRole.FromType(roleType);
        var getProjectsUseCase = Substitute.For<IGetProjectsUseCase>();
        var creationDate = DateTime.Parse("2024-02-08");
        getProjectsUseCase.GetAllProjects(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<ProjectDetails>
            {
                new(ProjectId.Create("id"), "name", creationDate)
            }));

        var response = await _api.GetApi(new TestWebApplicationFactory<Program>(outputHelper, [role],
            services => { services.AddSingleton(getProjectsUseCase); }));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var getProjectListResponse = await response.ReadContentAsync<WebGetProjectListResponse>();
        getProjectListResponse.ShouldBeEquivalentTo(
            new WebGetProjectListResponse([new WebGetProjectDetailsResponse("id", "name", creationDate)])
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
