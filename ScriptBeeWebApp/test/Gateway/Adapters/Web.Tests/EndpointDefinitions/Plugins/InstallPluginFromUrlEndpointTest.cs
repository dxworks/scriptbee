using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Plugins;
using ScriptBee.Web.EndpointDefinitions.Plugins.Contracts;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Plugins;

using InstallPluginResult = OneOf<
    ProjectDetails,
    ProjectDoesNotExistsError,
    PluginManifestNotFoundError,
    PluginInstallationError
>;

public class InstallPluginFromUrlEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/plugins/url";

    [Fact]
    public async Task GivenUrlPlugin_WhenSuccessful_ShouldReturnOkWithProjectDetails()
    {
        var projectId = ProjectId.FromValue("project-id");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPluginAsync(
                projectId,
                "https://example.com/plugin.zip",
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<InstallPluginResult>(projectDetails));

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new WebInstallPluginFromUrlRequest("https://example.com/plugin.zip")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        content.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GivenUrlPlugin_WhenProjectDoesNotExist_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPluginAsync(
                projectId,
                "https://example.com/plugin.zip",
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<InstallPluginResult>(new ProjectDoesNotExistsError(projectId))
            );

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new WebInstallPluginFromUrlRequest("https://example.com/plugin.zip")
        );

        await AssertProjectNotFoundProblem(response, TestUrl);
    }

    [Fact]
    public async Task GivenUrlPlugin_WhenManifestNotFound_ShouldReturnBadRequest()
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPluginAsync(
                projectId,
                "https://example.com/plugin.zip",
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<InstallPluginResult>(new PluginManifestNotFoundError()));

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new WebInstallPluginFromUrlRequest("https://example.com/plugin.zip")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertBadRequestProblem(
            response.Content,
            TestUrl,
            "Plugin Manifest Not Found",
            "The 'manifest.yaml' file was not found at the root of the plugin."
        );
    }

    [Fact]
    public async Task GivenUrlPlugin_WhenInstallationError_ShouldReturnInternalServerError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var pluginId = new PluginId("plugin-id", new Version("1.2.3"));
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPluginAsync(
                projectId,
                "https://example.com/plugin.zip",
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<InstallPluginResult>(new PluginInstallationError(pluginId, []))
            );

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new WebInstallPluginFromUrlRequest("https://example.com/plugin.zip")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        await AssertInternalServerErrorProblem(
            response.Content,
            TestUrl,
            "Plugin Installation Failed",
            "Could not install plugin plugin-id with version '1.2.3'."
        );
    }
}
