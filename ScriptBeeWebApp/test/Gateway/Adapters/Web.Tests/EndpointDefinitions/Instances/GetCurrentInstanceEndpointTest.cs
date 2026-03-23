using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Instances.Contracts;
using static ScriptBee.Tests.Common.InstanceInfoFixture;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Instances;

public class GetCurrentInstanceEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/project-id/instances/current";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnInstanceInfoWithContext()
    {
        var useCase = Substitute.For<IGetCurrentInstanceUseCase>();
        var projectId = ProjectId.FromValue("project-id");
        var instanceInfo = BasicInstanceInfo(projectId);
        useCase
            .GetCurrentInstance(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, NoInstanceAllocatedForProjectError>>(
                    instanceInfo
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
        var webProjectInstanceInfo = await response.ReadContentAsync<WebProjectInstance>();
        webProjectInstanceInfo.Id.ShouldBe(instanceInfo.Id.ToString());
        webProjectInstanceInfo.CreationDate.ShouldBe(instanceInfo.CreationDate);
    }

    [Fact]
    public async Task NoInstanceAllocatedForProject_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<IGetCurrentInstanceUseCase>();
        var projectId = ProjectId.FromValue("project-id");
        useCase
            .GetCurrentInstance(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, NoInstanceAllocatedForProjectError>>(
                    new NoInstanceAllocatedForProjectError(projectId)
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

        await AssertNoInstanceAllocatedForProjectNotFoundProblem(response, TestUrl);
    }
}
