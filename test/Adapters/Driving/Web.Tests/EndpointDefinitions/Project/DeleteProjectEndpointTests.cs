using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Project;
using Xunit.Abstractions;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Project;

public class DeleteProjectEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id";
    private readonly TestApiCaller _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnNoContent()
    {
        var deleteProjectUseCase = Substitute.For<IDeleteProjectUseCase>();
        deleteProjectUseCase
            .DeleteProject(
                new DeleteProjectCommand(ProjectId.FromValue("id")),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<OneOf<Unit>>(new Unit()));

        var response = await _api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(deleteProjectUseCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
