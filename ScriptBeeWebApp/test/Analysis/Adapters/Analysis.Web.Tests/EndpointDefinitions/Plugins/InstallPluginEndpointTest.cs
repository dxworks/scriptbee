using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Plugin;
using ScriptBee.UseCases.Plugin.Errors;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Plugins;

public class InstallPluginEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/plugins";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldInstallPlugin_WithValidCommand()
    {
        const string pluginId = "test-plugin";
        const string version = "1.0.0";

        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPlugin(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Success, InvalidPluginError, PluginInstallationError>>(
                    new Success()
                )
            );

        var response = await _api.PostApi(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebInstallPluginCommand(pluginId, version)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await useCase.Received(1).InstallPlugin(pluginId, version, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenPluginIdIsEmpty()
    {
        var useCase = Substitute.For<IInstallPluginUseCase>();

        var response = await _api.PostApi(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebInstallPluginCommand("", "1.0.0")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenVersionIsEmpty()
    {
        var useCase = Substitute.For<IInstallPluginUseCase>();

        var response = await _api.PostApi(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebInstallPluginCommand("test-plugin", "")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenInvalidPluginError()
    {
        const string pluginId = "test-plugin";
        const string version = "invalid-version";

        var invalidPluginError = new InvalidPluginError(pluginId, version);
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPlugin(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Success, InvalidPluginError, PluginInstallationError>>(
                    invalidPluginError
                )
            );

        var response = await _api.PostApi(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebInstallPluginCommand(pluginId, version)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnInternalServerError_WhenPluginInstallationError()
    {
        const string pluginId = "test-plugin";
        const string version = "1.0.0";

        var installError = new PluginInstallationError(pluginId, version);
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase
            .InstallPlugin(pluginId, version, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<Success, InvalidPluginError, PluginInstallationError>>(
                    installError
                )
            );

        var response = await _api.PostApi(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebInstallPluginCommand(pluginId, version)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }
}
