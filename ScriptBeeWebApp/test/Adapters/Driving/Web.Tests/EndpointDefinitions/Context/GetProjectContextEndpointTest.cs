using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context;

public class GetProjectContextEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/b50d1f67-de23-45ea-ad99-f844be49e450/context";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnContextSlices()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("b50d1f67-de23-45ea-ad99-f844be49e450");
        var useCase = Substitute.For<IGetInstanceContextUseCase>();
        useCase
            .Get(new GetInstanceContextQuery(projectId, instanceId), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<IEnumerable<ContextSlice>, InstanceDoesNotExistsError>>(
                    new List<ContextSlice> { new("model", ["plugin-id"]) }
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var contextResponse = await response.ReadContentAsync<
            IEnumerable<WebProjectContextSlice>
        >();
        var webProjectContextSlice = contextResponse.ToList().Single();
        webProjectContextSlice.Model.ShouldBe("model");
        webProjectContextSlice.PluginIds.ShouldBeEquivalentTo(new List<string> { "plugin-id" });
    }

    [Fact]
    public async Task InstanceNotExists_ShouldReturnNotFound()
    {
        var instanceId = new InstanceId("b50d1f67-de23-45ea-ad99-f844be49e450");
        var useCase = Substitute.For<IGetInstanceContextUseCase>();
        useCase
            .Get(Arg.Any<GetInstanceContextQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<IEnumerable<ContextSlice>, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await AssertInstanceNotFoundProblem(
            response,
            TestUrl,
            "b50d1f67-de23-45ea-ad99-f844be49e450"
        );
    }
}
