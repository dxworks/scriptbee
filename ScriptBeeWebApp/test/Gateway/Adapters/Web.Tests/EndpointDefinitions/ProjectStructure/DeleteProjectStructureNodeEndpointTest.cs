using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.ProjectStructure;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.ProjectStructure;

public class DeleteProjectStructureNodeEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id/files/d67bbdb2-6e65-45b0-8947-41dbee53093c";

    [Fact]
    public async Task DeleteIsSuccessful()
    {
        var useCase = Substitute.For<IDeleteProjectFilesUseCase>();
        useCase
            .Delete(
                new DeleteFileCommand(
                    ProjectId.FromValue("id"),
                    new ScriptId("d67bbdb2-6e65-45b0-8947-41dbee53093c")
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(new Success());

        var api = new TestApiCaller<Program>(TestUrl);
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GivenProjectDoesNotExistsError_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("id");
        var useCase = Substitute.For<IDeleteProjectFilesUseCase>();
        useCase
            .Delete(
                new DeleteFileCommand(
                    projectId,
                    new ScriptId("d67bbdb2-6e65-45b0-8947-41dbee53093c")
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<Success, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        var api = new TestApiCaller<Program>(TestUrl);
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await AssertProjectNotFoundProblem(response, TestUrl, projectId.ToString());
    }
}
