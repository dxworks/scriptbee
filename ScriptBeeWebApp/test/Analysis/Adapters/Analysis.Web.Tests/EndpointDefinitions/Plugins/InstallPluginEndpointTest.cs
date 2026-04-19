using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf.Types;
using ScriptBee.Analysis.Web.EndpointDefinitions.Plugins.Contracts;
using ScriptBee.Domain.Model.Plugins;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;
using ScriptBee.UseCases.Analysis.Errors;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Plugins;

public class InstallPluginEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/plugins";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldInstallPlugin_WithValidCommand()
    {
        var pluginId = new PluginId("test-plugin", new Version("1.0.0"));
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase.InstallPlugin(pluginId).Returns(new Success());

        var response = await _api.PostApi(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebInstallPluginCommand("test-plugin", "1.0.0")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        useCase.Received(1).InstallPlugin(pluginId);
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
        var pluginId = new PluginId("test-plugin", new Version("1.0.0"));

        var invalidPluginError = new InvalidPluginError(pluginId);
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase.InstallPlugin(pluginId).Returns(invalidPluginError);

        var response = await _api.PostApi(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebInstallPluginCommand("test-plugin", "1.0.0")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnInternalServerError_WhenPluginInstallationError()
    {
        var pluginId = new PluginId("test-plugin", new Version("1.0.0"));
        var installError = new PluginInstallationError(pluginId);
        var useCase = Substitute.For<IInstallPluginUseCase>();
        useCase.InstallPlugin(pluginId).Returns(installError);

        var response = await _api.PostApi(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebInstallPluginCommand("test-plugin", "1.0.0")
        );

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }
}
