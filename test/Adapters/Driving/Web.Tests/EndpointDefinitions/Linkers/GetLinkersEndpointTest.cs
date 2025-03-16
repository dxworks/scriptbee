using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using Xunit.Abstractions;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Linkers;

public class GetLinkersEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/9c31916d-3265-4a94-9112-a5b0f1adc14c/linkers";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnInstanceLinkers()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("9c31916d-3265-4a94-9112-a5b0f1adc14c");
        var useCase = Substitute.For<IGetInstanceLinkersUseCase>();
        useCase
            .Get(new GetLinkersQuery(projectId, instanceId), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IEnumerable<Linker>>([new Linker("linker-id", "linker-name")])
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
        var linkersResponse = await response.ReadContentAsync<IEnumerable<WebLinker>>();
        linkersResponse
            .ToList()
            .ShouldBeEquivalentTo(new List<WebLinker> { new("linker-id", "linker-name") });
    }
}
