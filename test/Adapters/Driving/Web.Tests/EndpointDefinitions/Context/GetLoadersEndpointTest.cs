using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using Xunit.Abstractions;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context;

public class GetLoadersEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/2f3423e1-1450-45d0-a081-84656700a174/loaders";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnInstanceLoaders()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("2f3423e1-1450-45d0-a081-84656700a174");
        var useCase = Substitute.For<IGetInstanceLoadersUseCase>();
        useCase
            .Get(new GetLoadersQuery(projectId, instanceId), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Loader>>([new Loader("loader-id", "loader-name")])
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
        var loadersResponse = await response.ReadContentAsync<IEnumerable<WebLoader>>();
        loadersResponse
            .ToList()
            .ShouldBeEquivalentTo(new List<WebLoader> { new("loader-id", "loader-name") });
    }
}
