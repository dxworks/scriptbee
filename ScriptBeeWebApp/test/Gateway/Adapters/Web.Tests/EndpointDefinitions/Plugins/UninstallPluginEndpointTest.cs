using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Plugins;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Plugins;

public class UninstallPluginEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/plugins/plugin-id";

    [Fact]
    public async Task ShouldBadRequest_WhenPluginVersionIsNotSupplied()
    {
        var useCase = Substitute.For<IUninstallPluginUseCase>();

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnNoContent_WhenPluginInstalledSuccessfully()
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IUninstallPluginUseCase>();
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);
        useCase
            .UninstallPluginAsync(
                new UninstallPluginCommand(projectId, "plugin-id", "1.2.3"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        // Act
        TestApiCaller<Program> api = new($"{TestUrl}?version=1.2.3");
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GivenProjectDoesNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IUninstallPluginUseCase>();
        useCase
            .UninstallPluginAsync(
                new UninstallPluginCommand(projectId, "plugin-id", "1.2.3"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
                )
            );

        TestApiCaller<Program> api = new($"{TestUrl}?version=1.2.3");
        var response = await api.DeleteApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await AssertProjectNotFoundProblem(response, TestUrl);
    }
}
