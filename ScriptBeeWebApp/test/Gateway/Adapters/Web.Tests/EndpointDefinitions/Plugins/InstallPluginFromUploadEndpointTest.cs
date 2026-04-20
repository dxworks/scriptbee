using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Gateway.Plugins;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Plugins;

using InstallPluginResult = OneOf<
    ProjectDetails,
    ProjectDoesNotExistsError,
    PluginManifestNotFoundError,
    PluginAlreadyExistsError,
    PluginInstallationError
>;

public class InstallPluginFromUploadEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/plugins";

    [Fact]
    public async Task GivenUploadPlugin_WhenSuccessful_ShouldReturnOkWithProjectDetails()
    {
        var projectId = ProjectId.FromValue("project-id");
        var projectDetails = ProjectDetailsFixture.BasicProjectDetails(projectId);
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPluginAsync(projectId, Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<InstallPluginResult>(projectDetails));

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApiFormWithFile(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new Dictionary<string, string>(),
            new Dictionary<string, byte[]> { { "file", [1, 2, 3] } }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );
        content.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GivenUploadPlugin_WhenProjectDoesNotExist_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPluginAsync(projectId, Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<InstallPluginResult>(new ProjectDoesNotExistsError(projectId))
            );

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApiFormWithFile(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new Dictionary<string, string>(),
            new Dictionary<string, byte[]> { { "file", [1, 2, 3] } }
        );

        await AssertProjectNotFoundProblem(response, TestUrl);
    }

    [Fact]
    public async Task GivenUploadPlugin_WhenManifestNotFound_ShouldReturnBadRequest()
    {
        var projectId = ProjectId.FromValue("project-id");
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPluginAsync(projectId, Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<InstallPluginResult>(new PluginManifestNotFoundError()));

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApiFormWithFile(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new Dictionary<string, string>(),
            new Dictionary<string, byte[]> { { "file", [1, 2, 3] } }
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
    public async Task GivenUploadPlugin_WhenPluginAlreadyExists_ShouldReturnConflict()
    {
        var projectId = ProjectId.FromValue("project-id");
        var pluginId = new PluginId("plugin-id", new Version("1.2.3"));
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPluginAsync(projectId, Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<InstallPluginResult>(new PluginAlreadyExistsError(pluginId)));

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApiFormWithFile(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new Dictionary<string, string>(),
            new Dictionary<string, byte[]> { { "file", [1, 2, 3] } }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        await AssertConflictProblem(
            response.Content,
            TestUrl,
            "Plugin Already Exists",
            "A plugin with the ID 'plugin-id' and version '1.2.3' already exists."
        );
    }

    [Fact]
    public async Task GivenUploadPlugin_WhenInstallationError_ShouldReturnInternalServerError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var pluginId = new PluginId("plugin-id", new Version("1.2.3"));
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPluginAsync(projectId, Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<InstallPluginResult>(new PluginInstallationError(pluginId, []))
            );

        TestApiCaller<Program> api = new(TestUrl);
        var response = await api.PostApiFormWithFile(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services => services.AddSingleton(useCase)
            ),
            new Dictionary<string, string>(),
            new Dictionary<string, byte[]> { { "file", [1, 2, 3] } }
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
