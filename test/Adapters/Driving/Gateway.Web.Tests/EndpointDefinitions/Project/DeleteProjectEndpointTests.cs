using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Authorization;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Driving.UseCases.Project;
using Shouldly;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests.EndpointDefinitions.Project;

public class DeleteProjectEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/scriptbee/projects/id";
    private readonly TestApiCaller _api = new(TestUrl);

    [Fact]
    public async Task AdministratorRole_ShouldReturnNoContent()
    {
        var deleteProjectUseCase = Substitute.For<IDeleteProjectUseCase>();
        deleteProjectUseCase
            .DeleteProject(new DeleteProjectCommand(ProjectId.FromValue("id")), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Unit>>(new Unit()));

        var response =
            await _api.DeleteApi(new TestWebApplicationFactory<Program>(outputHelper, [UserRole.Administrator],
                services => { services.AddSingleton(deleteProjectUseCase); }));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task OtherRole_ShouldReturnForbidden()
    {
        var response = await _api.DeleteApi(new TestWebApplicationFactory<Program>(outputHelper, [UserRole.Guest]));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task NoRoles_ShouldReturnForbidden()
    {
        var response = await _api.DeleteApi(new TestWebApplicationFactory<Program>(outputHelper, []));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UnauthorizedUser_ShouldReturnUnauthorized()
    {
        var response = await _api.DeleteApiWithoutAuthorization(new TestWebApplicationFactory<Program>(outputHelper));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
